using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoTi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // 🔹 Dashboard inicial
        [HttpGet]
        public IActionResult Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            var usuarioNome = HttpContext.Session.GetString("NomeUsuario") ?? "Usuário";
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";

            ViewBag.NomeUsuario = usuarioNome;
            ViewBag.Papel = papel;

            var chamados = _chamadoRepo.ListarTodosChamados();

            foreach (var c in chamados)
                c.NomeUsuario ??= "Usuário";

            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return View("~/Views/DashboardTecnico/Index.cshtml", chamados);

            return View("~/Views/Home/Dashboard.cshtml", chamados);
        }

        // 🔹 Criar chamado
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
                DataAbertura = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
    )
            };

            _chamadoRepo.CriarChamado(chamado);
            TempData["MensagemSucesso"] = "Chamado criado com sucesso!";
            return RedirectToAction("Index");
        }

        // 🔹 Fechar chamado
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

        // 🔹 Filtro de chamados
        [HttpPost]
        public IActionResult PesquisarChamados(string id, string assunto, string data, string prioridade, string colaborador)
        {
            var chamados = _chamadoRepo.ListarTodosChamados();
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";

            var resultados = new List<Chamado>();

            foreach (var c in chamados)
            {
                c.NomeUsuario ??= "Usuário";

                bool match =
                    (string.IsNullOrEmpty(id) || c.Id.ToString().Contains(id)) &&
                    (string.IsNullOrEmpty(assunto) || c.Titulo.Contains(assunto, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(data) || c.DataAbertura.ToString("yyyy-MM-dd").Contains(data)) &&
                    (string.IsNullOrEmpty(colaborador) || c.NomeUsuario.Contains(colaborador, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(prioridade) || c.Status.Contains(prioridade, StringComparison.OrdinalIgnoreCase));

                if (match)
                    resultados.Add(c);
            }

            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return View("~/Views/DashboardTecnico/Index.cshtml", resultados);

            return View("~/Views/Home/Dashboard.cshtml", resultados);
        }

        // ====== Gerenciamento de usuários (somente técnico) ======

        // 🔹 Lista todos os usuários
        [HttpGet]
        public IActionResult Usuarios()
        {
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            if (!papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            var usuarios = _usuarioRepo.ListarUsuarios();
            return View("~/Views/DashboardTecnico/Usuarios.cshtml", usuarios);
        }

        // 🔹 Cria novo usuário (feito pelo técnico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarUsuarioPeloTecnico(string Nome, string Email, string Senha, string Papel)
        {
            var papelSess = HttpContext.Session.GetString("Papel") ?? "colaborador";
            if (!papelSess.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                TempData["UsuarioMsgErro"] = "Nome, email e senha são obrigatórios.";
                return RedirectToAction("Usuarios");
            }

            var novo = new Usuario
            {
                Nome = Nome.Trim(),
                Email = Email.Trim(),
                Senha = Senha,
                Papel = string.IsNullOrWhiteSpace(Papel) ? "colaborador" : Papel
            };

            var ok = _usuarioRepo.CriarUsuario(novo);
            TempData[ok ? "UsuarioMsgSucesso" : "UsuarioMsgErro"] =
                ok ? "Usuário criado com sucesso." : "Falha ao criar usuário (verifique email duplicado).";

            return RedirectToAction("Usuarios");
        }

        // 🔹 Exclui usuário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirUsuario(int id)
        {
            var papelSess = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioIdSess = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            if (!papelSess.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            if (id == usuarioIdSess)
            {
                TempData["UsuarioMsgErro"] = "Você não pode excluir sua própria conta enquanto estiver logado.";
                return RedirectToAction("Usuarios");
            }

            var ok = _usuarioRepo.ExcluirUsuario(id);
            TempData[ok ? "UsuarioMsgSucesso" : "UsuarioMsgErro"] =
                ok ? "Usuário excluído com sucesso." : "Falha ao excluir usuário.";

            return RedirectToAction("Usuarios");
        }
    }
}
