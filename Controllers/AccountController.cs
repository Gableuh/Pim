using Microsoft.AspNetCore.Mvc;

namespace ProjetoTi.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            // Aqui futuramente você validará com Supabase
            if (email == "admin@teste.com" && senha == "123")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Email ou senha inválidos!";
            return View();
        }
    }
}
