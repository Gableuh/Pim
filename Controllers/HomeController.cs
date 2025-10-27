using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class HomeController : Controller
    {
        private ChamadoRepository _chamadoRepo = new ChamadoRepository();

        // Dashboard - lista os chamados do usuário
        public IActionResult Dashboard()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chamados = _chamadoRepo.ListarChamadosPorUsuario(usuarioId.Value);
            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome");
            return View(chamados);
        }


        // Criar chamado
        [HttpGet]
        public IActionResult CriarChamado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CriarChamado(string titulo, string descricao)
        {
            if (TempData["UsuarioId"] == null)
                return RedirectToAction("Index", "Login");

            int usuarioId = (int)TempData["UsuarioId"];

            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto",
                UsuarioId = usuarioId,
                DataCriacao = DateTime.Now
            };

            _chamadoRepo.CriarChamado(chamado);
            return RedirectToAction("Dashboard");
        }

        // Atualizar status
        public IActionResult FecharChamado(int id)
        {
            _chamadoRepo.AtualizarStatus(id, "Fechado");
            return RedirectToAction("Dashboard");
        }
    }
}
