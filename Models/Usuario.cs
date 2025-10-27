using System;

namespace ProjetoTi.Models
{
    public class Usuario
    {
        // id é uuid no banco -> Guid aqui
        public Guid Id { get; set; } = Guid.NewGuid();

        // inicialize strings para evitar warnings CS8618
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Papel { get; set; } = "colaborador";
    }
}
