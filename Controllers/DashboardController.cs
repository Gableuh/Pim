using Microsoft.AspNetCore.Mvc;

namespace ProjetoTi.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/
        public IActionResult Index()
        {
            // Aqui poderemos futuramente buscar os chamados do usuário logado
            ViewBag.UsuarioNome = "User"; // temporário
            return View();
        }

        // GET: /Dashboard/Logout
        public IActionResult Logout()
        {
            // Depois faremos a limpeza da sessão e redirecionamento
            return RedirectToAction("Index", "Login");
        }
    }
}
