using Npgsql;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;
using BCrypt.Net; // Biblioteca para criptografia de senhas

namespace ProjetoTi.Data
{
    public class UsuarioRepository
    {
        // 🔹 String de conexão com o banco Supabase (PostgreSQL)
        private readonly string connectionString =
            "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;" +
            "Port=5432;Database=postgres;" +
            "Username=postgres;Password=ProjetoTi123;" +
            "SSL Mode=Require;Trust Server Certificate=true;";

        // 🔹 Autentica usuário (verifica email e senha)
        public Usuario? Autenticar(string email, string senha)
        {
            // Validação inicial — impede campos vazios
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                throw new ArgumentException("Email e senha não podem ser vazios.");

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Busca o usuário pelo email
            using var cmd = new NpgsqlCommand(
                "SELECT id, nome, email, senha, papel, criado_em FROM usuarios WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Recupera a senha armazenada no banco
                string senhaHash = reader.GetString(reader.GetOrdinal("senha"));
                bool senhaConfere = false;

                // Detecta se a senha armazenada está criptografada com BCrypt
                if (senhaHash.StartsWith("$2a$") || senhaHash.StartsWith("$2b$"))
                {
                    // Se for hash, verifica usando o BCrypt
                    senhaConfere = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
                }
                else
                {
                    // Se for texto puro (usuário antigo), compara direto
                    senhaConfere = senha == senhaHash;
                }

                // Se a senha estiver correta, retorna o objeto do usuário autenticado
                if (senhaConfere)
                {
                    return new Usuario
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Nome = reader.GetString(reader.GetOrdinal("nome")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Senha = senhaHash, // Senha criptografada
                        Papel = reader.GetString(reader.GetOrdinal("papel"))
                    };
                }
            }

            // Caso não encontre ou senha inválida, retorna null
            return null;
        }

        // 🔹 Cria novo usuário no banco (com senha criptografada)
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
                // Executa o comando e retorna true se der certo
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (PostgresException ex)
            {
                // Exibe o erro no console caso ocorra
                Console.WriteLine("Erro ao criar usuário: " + ex.Message);
                return false;
            }
        }

        // 🔹 Cria usuário técnico padrão para testes
        public void CriarUsuarioTeste()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Verifica se já existe o usuário admin
            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM usuarios WHERE email=@e", conn);
            checkCmd.Parameters.AddWithValue("@e", "admin@admin.com");
            long count = (long)checkCmd.ExecuteScalar()!;
            if (count > 0) return; // Se já existir, não cria novamente

            // Gera hash da senha padrão
            string senhaHash = BCrypt.Net.BCrypt.HashPassword("12345");

            // Cria o usuário técnico admin
            using var insertCmd = new NpgsqlCommand(
                "INSERT INTO usuarios (nome, email, senha, papel, criado_em) VALUES (@n, @e, @s, @p, NOW())", conn);
            insertCmd.Parameters.AddWithValue("@n", "Admin");
            insertCmd.Parameters.AddWithValue("@e", "admin@admin.com");
            insertCmd.Parameters.AddWithValue("@s", senhaHash);
            insertCmd.Parameters.AddWithValue("@p", "tecnico");
            insertCmd.ExecuteNonQuery();
        }

        // 🔹 Lista todos os usuários cadastrados (usado no painel do técnico)
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
                    Papel = reader.IsDBNull(reader.GetOrdinal("papel"))
                        ? "colaborador" // Define padrão caso o campo venha nulo
                        : reader.GetString(reader.GetOrdinal("papel"))
                });
            }

            return lista;
        }

        // 🔹 Exclui usuário do sistema pelo ID
        public bool ExcluirUsuario(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = "DELETE FROM usuarios WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                // Retorna true se o comando afetar pelo menos uma linha
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                // Mostra erro no console caso falhe
                Console.WriteLine("Erro ao excluir usuário: " + ex.Message);
                return false;
            }
        }
    }
}
