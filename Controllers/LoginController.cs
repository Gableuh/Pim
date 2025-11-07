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
            return View(); // Exibe tela de login
        }

        [HttpPost]
        public IActionResult Index(string email, string password, string role)
        {
            // Mantém os valores digitados caso o login falhe
            ViewBag.Email = email;
            ViewBag.Role = role;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.MensagemErro = "Todos os campos devem estar preenchidos.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }

            var usuario = _repo.Autenticar(email, password);
            if (usuario == null)
            {
                ViewBag.MensagemErro = "Usuário ou senha inválidos.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }

            if (!string.Equals(usuario.Papel, role, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.MensagemErro = "Email cadastrado já tem um login diferente.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }


            // Armazena informações na sessão
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("Papel", usuario.Papel);
            HttpContext.Session.SetString("NomeUsuario", usuario.Nome);

            // Redireciona para a página inicial
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
