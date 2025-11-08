using Npgsql;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;
using BCrypt.Net; // Biblioteca para criptografia

namespace ProjetoTi.Data
{
    public class UsuarioRepository
    {
        private readonly string connectionString =
            "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;" +
            "Port=5432;Database=postgres;" +
            "Username=postgres;Password=ProjetoTi123;" +
            "SSL Mode=Require;Trust Server Certificate=true;";

        // 🔹 Autentica usuário (aceita senha pura e senha hash)
        public Usuario? Autenticar(string email, string senha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                throw new ArgumentException("Email e senha não podem ser vazios.");

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT id, nome, email, senha, papel, criado_em FROM usuarios WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string senhaHash = reader.GetString(reader.GetOrdinal("senha"));
                bool senhaConfere = false;

                // Detecta se o campo senha é hash do BCrypt
                if (senhaHash.StartsWith("$2a$") || senhaHash.StartsWith("$2b$"))
                {
                    // Senha armazenada é hash
                    senhaConfere = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
                }
                else
                {
                    // Senha armazenada ainda é texto puro (usuário antigo)
                    senhaConfere = senha == senhaHash;
                }

                if (senhaConfere)
                {
                    return new Usuario
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Nome = reader.GetString(reader.GetOrdinal("nome")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Senha = senhaHash,
                        Papel = reader.GetString(reader.GetOrdinal("papel"))
                    };
                }
            }

            return null;
        }

        // 🔹 Cria usuário novo (com senha criptografada)
        public bool CriarUsuario(Usuario usuario)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Criptografa a senha antes de salvar
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            using var cmd = new NpgsqlCommand(
                "INSERT INTO usuarios (nome, email, senha, papel, criado_em) VALUES (@n, @e, @s, @p, NOW())", conn);
            cmd.Parameters.AddWithValue("@n", usuario.Nome);
            cmd.Parameters.AddWithValue("@e", usuario.Email);
            cmd.Parameters.AddWithValue("@s", senhaHash);
            cmd.Parameters.AddWithValue("@p", usuario.Papel);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (PostgresException ex)
            {
                Console.WriteLine("Erro ao criar usuário: " + ex.Message);
                return false;
            }
        }

        // 🔹 Cria usuário de teste (com senha criptografada)
        public void CriarUsuarioTeste()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM usuarios WHERE email=@e", conn);
            checkCmd.Parameters.AddWithValue("@e", "admin@admin.com");
            long count = (long)checkCmd.ExecuteScalar()!;
            if (count > 0) return;

            string senhaHash = BCrypt.Net.BCrypt.HashPassword("12345");

            using var insertCmd = new NpgsqlCommand(
                "INSERT INTO usuarios (nome, email, senha, papel, criado_em) VALUES (@n, @e, @s, @p, NOW())", conn);
            insertCmd.Parameters.AddWithValue("@n", "Admin");
            insertCmd.Parameters.AddWithValue("@e", "admin@admin.com");
            insertCmd.Parameters.AddWithValue("@s", senhaHash);
            insertCmd.Parameters.AddWithValue("@p", "tecnico");
            insertCmd.ExecuteNonQuery();
        }

        // 🔹 Listar todos os usuários (para painel do técnico)
        public List<Usuario> ListarUsuarios()
        {
            var lista = new List<Usuario>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = @"SELECT id, nome, email, papel FROM usuarios ORDER BY id DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Papel = reader.IsDBNull(reader.GetOrdinal("papel")) ? "colaborador" : reader.GetString(reader.GetOrdinal("papel"))
                });
            }

            return lista;
        }

        // 🔹 Excluir usuário por id
        public bool ExcluirUsuario(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = "DELETE FROM usuarios WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao excluir usuário: " + ex.Message);
                return false;
            }
        }
    }
}
