using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Layout = _LoginLayout.cshtml
        }

        [HttpPost]
        public IActionResult Index(string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.MensagemErro = "Todos os campos devem estar preenchidos.";
                return View();
            }

            var usuario = _repo.Autenticar(email, password);
            if (usuario == null)
            {
                ViewBag.MensagemErro = "Usuário ou senha inválidos.";
                return View();
            }

            if (!string.Equals(usuario.Papel, role, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.MensagemErro = "Email cadastrado já tem um login diferente.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("Papel", usuario.Papel);
            HttpContext.Session.SetString("NomeUsuario", usuario.Nome);

            // Redireciona para Index que agora escolhe a view correta
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
