using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;

namespace ProjetoTi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // 🔹 Dashboard inicial (agora: todos os chamados aparecem por padrão para qualquer usuário;
        // a view faz a segmentação "Meus Chamados" por nome)
        [HttpGet]
        public IActionResult Index()
        {
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            var usuarioNome = HttpContext.Session.GetString("NomeUsuario") ?? "Usuário";

            ViewBag.NomeUsuario = usuarioNome;

            // Sempre traz TODOS os chamados (técnico e usuário verão todos no painel principal).
            var chamados = _chamadoRepo.ListarTodosChamados();

            // Garante que NomeUsuario nunca seja nulo
            foreach (var c in chamados)
                c.NomeUsuario ??= "Usuário";

            // Se for técnico, renderiza a view técnica (se preferir manter uma view separada);
            // aqui por compatibilidade seguimos enviando cada perfil para sua view específica se desejar.
            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/DashboardTecnico/Index.cshtml", chamados);
            }

            // Para usuário comum, renderiza o dashboard do usuário (com todos os chamados no Model).
            return View("~/Views/Home/Dashboard.cshtml", chamados);
        }

        [HttpPost]
        public IActionResult CriarChamado(string titulo, string descricao)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            if (usuarioId == 0 || string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descricao))
            {
                TempData["MensagemErro"] = "Todos os campos são obrigatórios.";
                return RedirectToAction("Index");
            }

            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto",
                IdUsuario = usuarioId,
                DataAbertura = DateTime.Now
            };

            _chamadoRepo.CriarChamado(chamado);
            TempData["MensagemSucesso"] = "Chamado criado com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult FecharChamado(int id)
        {
            var chamado = _chamadoRepo.BuscarPorId(id);
            if (chamado == null)
            {
                TempData["MensagemErro"] = "Chamado não encontrado.";
                return RedirectToAction("Index");
            }

            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            // Só técnico pode fechar qualquer chamado; usuário só fecha os seus.
            if (!papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase) && chamado.IdUsuario != usuarioId)
            {
                TempData["MensagemErro"] = "Você não tem permissão para fechar este chamado.";
                return RedirectToAction("Index");
            }

            _chamadoRepo.AtualizarStatus(id, "Fechado");
            TempData["MensagemSucesso"] = "Chamado fechado com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PesquisarChamados(string id, string assunto, string data, string setor, string prioridade, string colaborador)
        {
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            if (!papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index"); // apenas técnicos usam esse endpoint no servidor

            var todosChamados = _chamadoRepo.ListarTodosChamados();
            var resultados = new List<Chamado>();

            foreach (var c in todosChamados)
            {
                c.NomeUsuario ??= "Usuário";

                bool match = (string.IsNullOrEmpty(id) || c.Id.ToString().Contains(id))
                             && (string.IsNullOrEmpty(assunto) || c.Titulo.Contains(assunto, StringComparison.OrdinalIgnoreCase))
                             && (string.IsNullOrEmpty(data) || c.DataAbertura.ToString("yyyy-MM-dd").Contains(data))
                             && (string.IsNullOrEmpty(colaborador) || c.NomeUsuario.Contains(colaborador, StringComparison.OrdinalIgnoreCase))
                             && (string.IsNullOrEmpty(prioridade) || c.Status.Contains(prioridade, StringComparison.OrdinalIgnoreCase));

                if (match)
                    resultados.Add(c);
            }

            return View("~/Views/DashboardTecnico/Index.cshtml", resultados);
        }
    }
}
