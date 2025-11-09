using Npgsql;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;

namespace ProjetoTi.Data
{
    public class ChamadoRepository
    {
        // 🔹 String de conexão com o banco de dados Supabase (PostgreSQL)
        // Inclui host, porta, usuário, senha e parâmetros de segurança
        private readonly string connectionString =
            "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;" +
            "Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

        // ================================================================
        // 🔹 Criar novo chamado
        // ================================================================
        public bool CriarChamado(Chamado chamado)
        {
            // Abre uma conexão com o banco usando Npgsql
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Comando SQL de inserção (INSERT) para criar um novo registro na tabela "chamados"
            var sql = @"
                INSERT INTO chamados (titulo, descricao, status, data_abertura, id_usuario, id_tecnico)
                VALUES (@t, @d, @s, @data, @u, @tec)";

            // Cria o comando SQL e define os parâmetros de forma segura
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", chamado.Titulo);      // título do chamado
            cmd.Parameters.AddWithValue("@d", chamado.Descricao);   // descrição detalhada
            cmd.Parameters.AddWithValue("@s", chamado.Status);      // status inicial (ex: "aberto")
            cmd.Parameters.AddWithValue("@u", chamado.IdUsuario);   // id do usuário que abriu
            cmd.Parameters.AddWithValue("@tec", (object?)chamado.IdTecnico ?? DBNull.Value); // técnico (opcional)

            // Define o horário de abertura ajustado para o fuso horário do Brasil
            cmd.Parameters.AddWithValue("@data",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
            );

            try
            {
                // Executa o comando no banco (não retorna resultado)
                cmd.ExecuteNonQuery();
                return true; // sucesso
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar chamado: {ex.Message}");
                return false; // falha
            }
        }

        // ================================================================
        // 🔹 Listar chamados de um usuário específico
        // ================================================================
        public List<Chamado> ListarChamadosPorUsuario(int idUsuario)
        {
            var lista = new List<Chamado>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Consulta todos os chamados criados por um determinado usuário
            var sql = @"
                SELECT id, titulo, descricao, status, data_abertura, id_usuario, id_tecnico
                FROM chamados
                WHERE id_usuario = @id
                ORDER BY data_abertura DESC"; // ordena do mais recente para o mais antigo

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idUsuario);

            // Lê os resultados do banco e converte em objetos Chamado
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
                    NomeUsuario = null // ainda não carregado
                });
            }

            return lista; // retorna a lista com todos os chamados do usuário
        }

        // ================================================================
        // 🔹 Listar todos os chamados (usado no painel do técnico)
        // ================================================================
        public List<Chamado> ListarTodosChamados()
        {
            var lista = new List<Chamado>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Consulta todos os chamados do sistema, juntando o nome do usuário que abriu
            var sql = @"
                SELECT c.id, c.titulo, c.descricao, c.status, c.data_abertura, 
                       c.id_usuario, c.id_tecnico, u.nome AS usuario_nome
                FROM chamados c
                LEFT JOIN usuarios u ON c.id_usuario = u.id
                ORDER BY c.data_abertura DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            // Lê cada registro retornado e preenche os campos do modelo Chamado
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

            return lista; // retorna a lista completa de chamados
        }

        // ================================================================
        // 🔹 Atualizar status de um chamado (por ID)
        // ================================================================
        public bool AtualizarStatus(int idChamado, string novoStatus)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Atualiza o status do chamado com base no ID
            var sql = "UPDATE chamados SET status = @s WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", novoStatus);
            cmd.Parameters.AddWithValue("@id", idChamado);

            try
            {
                // Executa o update e retorna true se pelo menos uma linha foi alterada
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar status do chamado: {ex.Message}");
                return false;
            }
        }

        // ================================================================
        // 🔹 Buscar um chamado específico pelo ID
        // ================================================================
        public Chamado? BuscarPorId(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Consulta apenas o chamado com o ID especificado
            var sql = @"
                SELECT id, titulo, descricao, status, data_abertura, id_usuario, id_tecnico
                FROM chamados
                WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Constrói o objeto Chamado com os dados retornados
                return new Chamado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "" : reader.GetString(reader.GetOrdinal("descricao")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    DataAbertura = reader.GetDateTime(reader.GetOrdinal("data_abertura")),
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    IdTecnico = reader.IsDBNull(reader.GetOrdinal("id_tecnico")) ? null : reader.GetInt32(reader.GetOrdinal("id_tecnico")),
                    NomeUsuario = null // não carrega nome aqui (apenas dados básicos)
                };
            }

            return null; // se não encontrar, retorna nulo
        }
    }
}
