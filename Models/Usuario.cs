using System;

namespace ProjetoTi.Models
{
    // 🔹 Classe que representa um usuário do sistema (colaborador ou técnico)
    public class Usuario
    {
        // 🔹 Identificador único do usuário (usado também na sessão)
        // O tipo int é compatível com HttpContext.Session.SetInt32()
        public int Id { get; set; }

        // 🔹 Nome completo do usuário (exibido em dashboards e relatórios)
        // Inicializado com string.Empty para evitar warnings de nulidade
        public string Nome { get; set; } = string.Empty;

        // 🔹 Endereço de e-mail do usuário (usado para login e identificação)
        public string Email { get; set; } = string.Empty;

        // 🔹 Senha do usuário (armazenada criptografada no banco)
        public string Senha { get; set; } = string.Empty;

        // 🔹 Papel do usuário no sistema (ex: "colaborador" ou "tecnico")
        // Define permissões e acesso a funcionalidades específicas
        public string Papel { get; set; } = "colaborador";
    }
}
