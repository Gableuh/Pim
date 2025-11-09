using Microsoft.AspNetCore.Mvc; // Importa o namespace necessário para usar controladores e ações no ASP.NET MVC

namespace ProjetoTi.Controllers
{
    // Controlador responsável por gerenciar as ações relacionadas à conta do usuário (login, logout, cadastro, etc.)
    public class AccountController : Controller
    {
        // Método que trata requisições GET para a página de Login
        // Exibe o formulário de login ao usuário
        public IActionResult Login()
        {
            return View(); // Retorna a View correspondente (Login.cshtml)
        }

        // Método que trata requisições POST para o login
        // É chamado quando o usuário envia o formulário com email e senha
        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            
            // Atualmente, faz apenas uma validação simples com valores fixos

            if (email == "admin@teste.com" && senha == "123")
            {
                // Se o email e senha estiverem corretos, redireciona o usuário para a Home (página principal)
                return RedirectToAction("Index", "Home");
            }

            // Caso o login falhe, define uma mensagem de erro que será exibida na View
            ViewBag.Erro = "Email ou senha inválidos!";

            // Retorna novamente a View de login para que o usuário tente outra vez
            return View();
        }
    }
}
