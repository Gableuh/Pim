using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class LoginController : Controller
    {
        private UsuarioRepository _repo = new UsuarioRepository();

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(string email, string senha)
        {
            var usuario = _repo.Autenticar(email, senha);
            if (usuario != null)
            {
                // Armazena o Id como string na sessão
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                HttpContext.Session.SetString("UsuarioNome", usuario.Nome);

                return RedirectToAction("Dashboard", "Home");
            }

            TempData["Erro"] = "Email ou senha inválidos.";
            return View();
        }

        // Método auxiliar para recuperar o Id do usuário da sessão
        private Guid? GetUsuarioId()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (Guid.TryParse(usuarioIdStr, out Guid usuarioId))
            {
                return usuarioId;
            }
            return null;
        }
    }
}
