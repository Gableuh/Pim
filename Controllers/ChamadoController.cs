using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;
using System;
using System.Linq;

namespace ProjetoTi.Controllers
{
    public class ChamadoController : Controller
    {
        private readonly ChamadoRepository _chamRepo;

        public ChamadoController(ChamadoRepository chamRepo)
        {
            _chamRepo = chamRepo;
        }

        // ===================== DASHBOARD =====================
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
                return RedirectToAction("Index", "Login");

            // Pega todos os chamados do usuário logado
            var chamados = _chamRepo.ListarChamadosPorUsuario(userId.Value);
            ViewBag.NomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            ViewBag.TotalChamados = chamados.Count();
            ViewBag.Abertos = chamados.Count(c => c.Status == "aberto");
            ViewBag.Fechados = chamados.Count(c => c.Status == "fechado");

            return View(chamados);
        }

        // ===================== LISTAR MEUS CHAMADOS =====================
        [HttpGet]
        public IActionResult Meus()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
                return RedirectToAction("Index", "Login");

            var meusChamados = _chamRepo.ListarChamadosPorUsuario(userId.Value);
            return View(meusChamados);
        }

        // ===================== CRIAR CHAMADO =====================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Titulo, string Setor, string Prioridade, string Colaborador, string Descricao)
        {
            var userIdObj = HttpContext.Session.GetInt32("UsuarioId");
            if (userIdObj == null)
            {
                TempData["MensagemErro"] = "Faça login antes de abrir um chamado.";
                return RedirectToAction("Index", "Login");
            }

            int userId = userIdObj.Value;

            var fullDesc = $"Setor: {Setor}\nPrioridade: {Prioridade}\nColaborador: {Colaborador}\n\n{Descricao}";

            var chamado = new Chamado
            {
                Titulo = Titulo?.Trim() ?? string.Empty,
                Descricao = fullDesc,
                Status = "aberto",
                IdUsuario = userId,
                DataAbertura = DateTime.Now
            };

            try
            {
                var ok = _chamRepo.CriarChamado(chamado);
                if (ok)
                {
                    return RedirectToAction("Success");
                }
                else
                {
                    ViewBag.MensagemErro = "Não foi possível abrir o chamado. Tente novamente.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Erro: " + ex.Message;
                return View();
            }
        }

        // ===================== TELA DE SUCESSO =====================
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
