using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;
using System;

namespace ProjetoTi.Controllers
{
    public class ChamadoController : Controller
    {
        private readonly ChamadoRepository _chamRepo;

        public ChamadoController(ChamadoRepository chamRepo)
        {
            _chamRepo = chamRepo;
        }

        // GET: /Chamado/Create  (abre o formulário)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Chamado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Titulo, string Setor, string Prioridade, string Colaborador, string Descricao)
        {
            // obtém usuário logado da sessão
            var userIdObj = HttpContext.Session.GetInt32("UsuarioId");
            if (userIdObj == null)
            {
                TempData["MensagemErro"] = "Faça login antes de abrir um chamado.";
                return RedirectToAction("Index", "Login");
            }

            int userId = userIdObj.Value;

            // Monta descrição agregando os campos extra (setor/prioridade/colaborador)
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

        // GET: /Chamado/Success
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
