using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class CadastroController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;

        public CadastroController()
        {
            _usuarioRepo = new UsuarioRepository();
        }

        // GET: /Cadastro
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Cadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Usuario model, string confirmPassword)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.Senha) || model.Senha != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "As senhas não coincidem ou estão vazias.");
                return View(model);
            }

            try
            {
                // opcional: aqui você pode aplicar hash na senha antes de salvar (recomendado)
                // model.Senha = BCrypt.Net.BCrypt.HashPassword(model.Senha);

                _usuarioRepo.CriarUsuario(model); // insere no Supabase
                TempData["Success"] = "Cadastro realizado com sucesso! Faça login para continuar.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao cadastrar usuário: " + ex.Message);
                return View(model);
            }
        }
    }
}
