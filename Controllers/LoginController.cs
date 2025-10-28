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
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password, string role)
        {
            try
            {
                // Validação básica
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
                {
                    ViewBag.MensagemErro = "Todos os campos devem estar preenchidos.";
                    return View();
                }

                // Autentica usuário
                var usuario = _repo.Autenticar(email, password);

                if (usuario == null)
                {
                    ViewBag.MensagemErro = "Usuário ou senha inválidos.";
                    return View();
                }

                // Verifica se o papel do usuário bate com o selecionado no login
                if (!string.Equals(usuario.Papel, role, StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.MensagemErro = "Email cadastrado já tem um login diferente.";
                    return View();
                }

                // Redireciona conforme papel
                if (usuario.Papel.ToLower() == "tecnico")
                {
                    return RedirectToAction("Index", "DashboardTecnico");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Erro inesperado: " + ex.Message;
                return View();
            }
        }
    }
}
