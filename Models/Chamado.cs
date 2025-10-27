namespace ProjetoTi.Models
{
    public class Chamado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Status { get; set; } = "Aberto"; // Aberto, Em andamento, Fechado
        public int UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
