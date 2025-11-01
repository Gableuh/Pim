using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();

        // Dashboard - lista chamados do usuário
        public IActionResult Dashboard()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chamados = _chamadoRepo.ListarChamadosPorUsuario(usuarioId.Value);
            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome");
            ViewData["Title"] = "Dashboard";

            return View(chamados); // Layout = _Layout
        }

        // GET: Abrir chamado
        [HttpGet]
        public IActionResult CriarChamado()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome");
            ViewData["Title"] = "Abrir Chamado";
            return View(); // Layout = _Layout
        }

        // POST: Abrir chamado
        [HttpPost]
        public IActionResult CriarChamado(string titulo, string descricao)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto",
                IdUsuario = usuarioId.Value,
                DataAbertura = DateTime.Now
            };

            _chamadoRepo.CriarChamado(chamado);
            return RedirectToAction("Dashboard");
        }

        // Fechar chamado
        public IActionResult FecharChamado(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            _chamadoRepo.AtualizarStatus(id, "Fechado");
            return RedirectToAction("Dashboard");
        }

        // Privacy
        public IActionResult Privacy()
        {
            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome") ?? "Usuário";
            ViewData["Title"] = "Privacy";
            return View(); // Layout = _Layout
        }
    }
}
