using System;

namespace ProjetoTi.Models
{
    public class Chamado
    {
        public int Id { get; set; }  // antes era Guid
        public int IdUsuario { get; set; }  // antes era Guid
        public int? IdTecnico { get; set; } // antes era Guid?
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Status { get; set; } = "aberto";
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime? DataFechamento { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;


    }
}
