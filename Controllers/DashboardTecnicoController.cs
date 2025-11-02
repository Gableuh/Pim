using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models; 

namespace ProjetoTi.Controllers
{
    public class DashboardTecnicoController : Controller
    {
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();

        // GET: /DashboardTecnico
        public IActionResult Index()
        {
            // simples: retorna todos os chamados para o técnico
            var chamados = _chamadoRepo.ListarTodosChamados();
            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome") ?? "Técnico";
            return View(chamados);
        }

        // método rápido para fechar chamado (puro post seria melhor)
        [HttpPost]
        public IActionResult Fechar(int id)
        {
            _chamadoRepo.AtualizarStatus(id, "Fechado");
            return RedirectToAction("Index");
        }
    }
}
