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

        // 🔹 Dashboard inicial (diferencia usuário e técnico pelo Papel)
        [HttpGet]
        public IActionResult Index()
        {
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            var usuarioNome = HttpContext.Session.GetString("NomeUsuario") ?? "Usuário";

            ViewBag.NomeUsuario = usuarioNome;

            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
            {
                var chamados = _chamadoRepo.ListarTodosChamados();
                foreach (var c in chamados)
                    c.NomeUsuario ??= "Usuário";

                // Redireciona para a view do técnico
                return View("~/Views/DashboardTecnico/Index.cshtml", chamados);
            }
            else
            {
                var chamados = _chamadoRepo.ListarChamadosPorUsuario(usuarioId);
                foreach (var c in chamados)
                    c.NomeUsuario ??= usuarioNome;

                // Redireciona para a view do usuário
                return View("~/Views/Home/Dashboard.cshtml", chamados);
            }
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
                return RedirectToAction("Index");

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
