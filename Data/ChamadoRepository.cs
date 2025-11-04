using Npgsql;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;

namespace ProjetoTi.Data
{
    public class ChamadoRepository
    {
        private readonly string connectionString =
            "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;" +
            "Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

        // 🔹 Criar novo chamado
        public bool CriarChamado(Chamado chamado)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = @"
                INSERT INTO chamados (titulo, descricao, status, data_abertura, id_usuario, id_tecnico)
                VALUES (@t, @d, @s, @data, @u, @tec)";


            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", chamado.Titulo);
            cmd.Parameters.AddWithValue("@d", chamado.Descricao);
            cmd.Parameters.AddWithValue("@s", chamado.Status);
            cmd.Parameters.AddWithValue("@u", chamado.IdUsuario);
            cmd.Parameters.AddWithValue("@tec", (object?)chamado.IdTecnico ?? DBNull.Value);

            // ✅ Hora correta do Brasil
            cmd.Parameters.AddWithValue("@data",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
            );

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar chamado: {ex.Message}");
                return false;
            }
        }

        // 🔹 Listar chamados de um usuário específico
        public List<Chamado> ListarChamadosPorUsuario(int idUsuario)
        {
            var lista = new List<Chamado>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = @"
                SELECT id, titulo, descricao, status, data_abertura, id_usuario, id_tecnico
                FROM chamados
                WHERE id_usuario = @id
                ORDER BY data_abertura DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idUsuario);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Chamado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "" : reader.GetString(reader.GetOrdinal("descricao")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    DataAbertura = reader.GetDateTime(reader.GetOrdinal("data_abertura")),
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    IdTecnico = reader.IsDBNull(reader.GetOrdinal("id_tecnico")) ? null : reader.GetInt32(reader.GetOrdinal("id_tecnico")),
                    NomeUsuario = null // inicializa como nulo
                });
            }

            return lista;
        }

        // 🔹 Listar todos os chamados (para painel técnico)
        public List<Chamado> ListarTodosChamados()
        {
            var lista = new List<Chamado>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = @"
                SELECT c.id, c.titulo, c.descricao, c.status, c.data_abertura, 
                       c.id_usuario, c.id_tecnico, u.nome AS usuario_nome
                FROM chamados c
                LEFT JOIN usuarios u ON c.id_usuario = u.id
                ORDER BY c.data_abertura DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Chamado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "" : reader.GetString(reader.GetOrdinal("descricao")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    DataAbertura = reader.GetDateTime(reader.GetOrdinal("data_abertura")),
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    IdTecnico = reader.IsDBNull(reader.GetOrdinal("id_tecnico")) ? null : reader.GetInt32(reader.GetOrdinal("id_tecnico")),
                    NomeUsuario = reader.IsDBNull(reader.GetOrdinal("usuario_nome")) ? "Usuário" : reader.GetString(reader.GetOrdinal("usuario_nome"))
                });
            }

            return lista;
        }

        // 🔹 Atualizar status por id
        public bool AtualizarStatus(int idChamado, string novoStatus)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = "UPDATE chamados SET status = @s WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", novoStatus);
            cmd.Parameters.AddWithValue("@id", idChamado);

            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar status do chamado: {ex.Message}");
                return false;
            }
        }

        // 🔹 Buscar chamado por ID
        public Chamado? BuscarPorId(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = @"
                SELECT id, titulo, descricao, status, data_abertura, id_usuario, id_tecnico
                FROM chamados
                WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Chamado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "" : reader.GetString(reader.GetOrdinal("descricao")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    DataAbertura = reader.GetDateTime(reader.GetOrdinal("data_abertura")),
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    IdTecnico = reader.IsDBNull(reader.GetOrdinal("id_tecnico")) ? null : reader.GetInt32(reader.GetOrdinal("id_tecnico")),
                    NomeUsuario = null // inicializa como nulo
                };
            }

            return null;
        }
    }
}
