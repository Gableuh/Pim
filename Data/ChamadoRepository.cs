using Npgsql;
using ProjetoTi.Models;


namespace ProjetoTi.Data
{
    public class ChamadoRepository
    {
        private string connectionString = "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

        // Listar todos os chamados de um usuário
        public List<Chamado> ListarChamadosPorUsuario(int usuarioId)
        {
            var lista = new List<Chamado>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT id, titulo, descricao, status, usuarioid, datacriacao FROM chamados WHERE usuarioid=@usuarioId ORDER BY datacriacao DESC", conn);
            cmd.Parameters.AddWithValue("usuarioId", usuarioId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Chamado
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    Descricao = reader.GetString(2),
                    Status = reader.GetString(3),
                    UsuarioId = reader.GetInt32(4),
                    DataCriacao = reader.GetDateTime(5)
                });
            }

            return lista;
        }

        // Criar um novo chamado
        public void CriarChamado(Chamado chamado)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("INSERT INTO chamados (titulo, descricao, status, usuarioid, datacriacao) VALUES (@titulo, @descricao, @status, @usuarioId, @dataCriacao)", conn);
            cmd.Parameters.AddWithValue("titulo", chamado.Titulo);
            cmd.Parameters.AddWithValue("descricao", chamado.Descricao);
            cmd.Parameters.AddWithValue("status", chamado.Status);
            cmd.Parameters.AddWithValue("usuarioId", chamado.UsuarioId);
            cmd.Parameters.AddWithValue("dataCriacao", chamado.DataCriacao);

            cmd.ExecuteNonQuery();
        }

        // Atualizar status do chamado
        public void AtualizarStatus(int chamadoId, string novoStatus)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("UPDATE chamados SET status=@status WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("status", novoStatus);
            cmd.Parameters.AddWithValue("id", chamadoId);

            cmd.ExecuteNonQuery();
        }

        // Buscar um chamado específico
        public Chamado? BuscarPorId(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT id, titulo, descricao, status, usuarioid, datacriacao FROM chamados WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Chamado
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    Descricao = reader.GetString(2),
                    Status = reader.GetString(3),
                    UsuarioId = reader.GetInt32(4),
                    DataCriacao = reader.GetDateTime(5)
                };
            }

            return null;
        }
    }
}
