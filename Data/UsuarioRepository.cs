using Npgsql;
using ProjetoTi.Models;

namespace ProjetoTi.Data
{
    public class UsuarioRepository
    {
        private string connectionString = "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

        // 🔹 Autentica usuário
        public Usuario? Autenticar(string email, string senha)
        {
            // ✅ Validação antes de conectar
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                throw new ArgumentException("Email e senha não podem ser vazios.");

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT id, nome, email, senha, papel, criado_em FROM usuarios WHERE email=@e AND senha=@s", conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Senha = reader.GetString(reader.GetOrdinal("senha")),
                    Papel = reader.GetString(reader.GetOrdinal("papel"))
                };
            }

            return null; // se não encontrar usuário
        }

        // 🔹 Cria usuário novo
        public bool CriarUsuario(Usuario usuario)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "INSERT INTO usuarios (nome, email, senha, papel, criado_em) VALUES (@n, @e, @s, @p, NOW())", conn);
            cmd.Parameters.AddWithValue("@n", usuario.Nome);
            cmd.Parameters.AddWithValue("@e", usuario.Email);
            cmd.Parameters.AddWithValue("@s", usuario.Senha);
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

        // 🔹 Cria usuário de teste
        public void CriarUsuarioTeste()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM usuarios WHERE email=@e", conn);
            checkCmd.Parameters.AddWithValue("@e", "gabriel@teste.com");
            long count = (long)checkCmd.ExecuteScalar()!;
            if (count > 0) return;

            using var insertCmd = new NpgsqlCommand(
                "INSERT INTO usuarios (nome, email, senha, papel, criado_em) VALUES (@n, @e, @s, @p, NOW())", conn);
            insertCmd.Parameters.AddWithValue("@n", "Gabriel Santos");
            insertCmd.Parameters.AddWithValue("@e", "gabriel@teste.com");
            insertCmd.Parameters.AddWithValue("@s", "123456");
            insertCmd.Parameters.AddWithValue("@p", "colaborador");
            insertCmd.ExecuteNonQuery();
        }
    }
}
