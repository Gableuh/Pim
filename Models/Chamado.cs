using System;

namespace ProjetoTi.Models
{
    // 🔹 Classe que representa um chamado (ticket) de suporte no sistema
    public class Chamado
    {
        // Identificador único do chamado (chave primária no banco)
        public int Id { get; set; }

        // ID do usuário (colaborador) que abriu o chamado
        public int IdUsuario { get; set; }

        // ID do técnico responsável (pode ser nulo enquanto não atribuído)
        public int? IdTecnico { get; set; }

        // Título curto que resume o problema ou solicitação
        public string Titulo { get; set; } = string.Empty;

        // Descrição detalhada do problema informado pelo usuário
        public string Descricao { get; set; } = string.Empty;

        // Status atual do chamado (ex: "aberto", "em andamento", "fechado")
        public string Status { get; set; } = "aberto";

        // Data e hora em que o chamado foi criado (padrão: UTC)
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

        // Data e hora em que o chamado foi encerrado (pode ser nula)
        public DateTime? DataFechamento { get; set; }

        // Nome do usuário que abriu o chamado (para exibição nas views)
        public string NomeUsuario { get; set; } = string.Empty;
    }
}
