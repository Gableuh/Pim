using Npgsql;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;

namespace ProjetoTi.Data
{
    public class ChamadoRepository
    {
        private readonly string connectionString =
            "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

        // 🔹 Cria um novo chamado
        public bool CriarChamado(Chamado chamado)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO chamados (titulo, descricao, status, data_abertura, id_usuario, id_tecnico)
                VALUES (@t, @d, @s, NOW(), @u, @tec)", conn);

            cmd.Parameters.AddWithValue("t", chamado.Titulo);
            cmd.Parameters.AddWithValue("d", chamado.Descricao);
            cmd.Parameters.AddWithValue("s", chamado.Status);
            cmd.Parameters.AddWithValue("u", chamado.IdUsuario);
            cmd.Parameters.AddWithValue("tec", (object?)chamado.IdTecnico ?? DBNull.Value);

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

        // 🔹 Lista chamados de um usuário específico
        public List<Chamado> ListarChamadosPorUsuario(int idUsuario)
        {
            var lista = new List<Chamado>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                SELECT id, titulo, descricao, status, data_abertura, id_usuario, id_tecnico
                FROM chamados
                WHERE id_usuario = @id
                ORDER BY data_abertura DESC", conn);

            cmd.Parameters.AddWithValue("id", idUsuario);

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
                    IdTecnico = reader.IsDBNull(reader.GetOrdinal("id_tecnico")) ? null : reader.GetInt32(reader.GetOrdinal("id_tecnico"))
                });
            }

            return lista;
        }

        // 🔹 Atualiza o status de um chamado
        public bool AtualizarStatus(int idChamado, string novoStatus)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("UPDATE chamados SET status = @s WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("s", novoStatus);
            cmd.Parameters.AddWithValue("id", idChamado);

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
    }
}
