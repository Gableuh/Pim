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
        public IActionResult Index(string email, string senha)
        {
            try
            {
                // tenta autenticar
                var usuario = _repo.Autenticar(email, senha);

                if (usuario == null)
                {
                    ViewBag.MensagemErro = "Usuário ou senha inválidos.";
                    return View();
                }

                if (usuario.Papel == "tecnico")
                {
                    //return View();
                }
               
                // se der certo, redireciona para dashboard (ou home)
                return RedirectToAction("Index", "Dashboard");
            }
            catch (ArgumentException ex)
            {
                // erro da validação: email/senha vazios
                ViewBag.MensagemErro = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                // outros erros (ex: conexão)
                ViewBag.MensagemErro = "Erro inesperado: " + ex.Message;
                return View();
            }
        }
    }
}
