using Microsoft.AspNetCore.Mvc; // Necessário para usar controladores e ações do ASP.NET MVC

namespace ProjetoTi.Controllers
{
    // Controlador responsável por exibir e gerenciar a tela principal (Dashboard) do sistema
    public class DashboardController : Controller
    {
        // ===============================================================
        // AÇÃO: Index (GET)
        // Exibe a página inicial do Dashboard do usuário logado
        // ===============================================================
        public IActionResult Index()
        {
            // Por enquanto, apenas define um nome de usuário temporário na ViewBag
            ViewBag.UsuarioNome = "User"; // Exemplo provisório até conectar com a autenticação real

            // Retorna a View "Index" localizada em Views/Dashboard/Index.cshtml
            return View();
        }

        // ===============================================================
        // AÇÃO: Logout (GET)
        // Responsável por encerrar a sessão do usuário e redirecioná-lo para o login
        // ===============================================================
        public IActionResult Logout()
        {
            // Após encerrar a sessão, o usuário será redirecionado para a tela de login
            return RedirectToAction("Index", "Login");
        }
    }
}
