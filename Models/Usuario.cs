using System;

namespace ProjetoTi.Models
{
    public class Usuario
    {
        // 🔹 ID é int para compatibilidade com HttpContext.Session.SetInt32
        public int Id { get; set; }

        // 🔹 Inicializa strings para evitar warnings CS8618
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Papel { get; set; } = "colaborador";
    }
}
