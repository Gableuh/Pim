//Esse controlador é o coração da aplicação, pois conecta a área do colaborador e do técnico, além de lidar com regras de permissão, CRUD de chamados e usuários.

using Microsoft.AspNetCore.Mvc; // Necessário para criar controladores e ações no ASP.NET MVC
using ProjetoTi.Data;           // Importa os repositórios de dados (ChamadoRepository, UsuarioRepository)
using ProjetoTi.Models;         // Importa as classes de modelo (Chamado, Usuario)
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoTi.Controllers
{
    // Controlador principal do sistema
    // Gerencia as operações da página inicial (dashboard), abertura e fechamento de chamados,
    // pesquisa e também o gerenciamento de usuários (no caso dos técnicos)
    public class HomeController : Controller
    {
        // Repositórios utilizados para acessar os dados de chamados e usuários
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // ===============================================================
        // AÇÃO: Index (GET)
        // Exibe a dashboard inicial, mostrando todos os chamados conforme o papel do usuário
        // ===============================================================
        [HttpGet]
        public IActionResult Index()
        {
            // Recupera dados do usuário logado na sessão
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            var usuarioNome = HttpContext.Session.GetString("NomeUsuario") ?? "Usuário";
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador"; // pode ser "colaborador" ou "técnico"

            // Passa dados para a view através da ViewBag
            ViewBag.NomeUsuario = usuarioNome;
            ViewBag.Papel = papel;

            // Busca todos os chamados no banco
            var chamados = _chamadoRepo.ListarTodosChamados();

            // Garante que todos os chamados tenham um nome de usuário preenchido
            foreach (var c in chamados)
                c.NomeUsuario ??= "Usuário";

            // Se o usuário for técnico, mostra o dashboard específico do técnico
            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return View("~/Views/DashboardTecnico/Index.cshtml", chamados);

            // Caso contrário, exibe o dashboard padrão do colaborador
            return View("~/Views/Home/Dashboard.cshtml", chamados);
        }

        // ===============================================================
        // AÇÃO: CriarChamado (POST)
        // Cria um novo chamado a partir dos dados enviados pelo formulário
        // ===============================================================
        [HttpPost]
        public IActionResult CriarChamado(string titulo, string descricao)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            // Valida os campos obrigatórios
            if (usuarioId == 0 || string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descricao))
            {
                TempData["MensagemErro"] = "Todos os campos são obrigatórios.";
                return RedirectToAction("Index");
            }

            // Cria o objeto Chamado com os dados do formulário
            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto", // Status inicial
                IdUsuario = usuarioId,
                DataAbertura = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time") // Converte para horário de Brasília
                )
            };

            // Salva o chamado no banco de dados
            _chamadoRepo.CriarChamado(chamado);

            TempData["MensagemSucesso"] = "Chamado criado com sucesso!";
            return RedirectToAction("Index");
        }

        // ===============================================================
        // AÇÃO: FecharChamado (GET)
        // Fecha um chamado existente (somente o dono ou um técnico pode fazer isso)
        // ===============================================================
        [HttpGet]
        public IActionResult FecharChamado(int id)
        {
            // Busca o chamado no banco pelo ID
            var chamado = _chamadoRepo.BuscarPorId(id);
            if (chamado == null)
            {
                TempData["MensagemErro"] = "Chamado não encontrado.";
                return RedirectToAction("Index");
            }

            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            // Verifica se o usuário tem permissão para fechar o chamado
            if (!papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase) && chamado.IdUsuario != usuarioId)
            {
                TempData["MensagemErro"] = "Você não tem permissão para fechar este chamado.";
                return RedirectToAction("Index");
            }

            // Atualiza o status para “Fechado”
            _chamadoRepo.AtualizarStatus(id, "Fechado");
            TempData["MensagemSucesso"] = "Chamado fechado com sucesso!";
            return RedirectToAction("Index");
        }

        // ===============================================================
        // AÇÃO: PesquisarChamados (POST)
        // Aplica filtros de busca sobre os chamados (por ID, título, data, etc.)
        // ===============================================================
        [HttpPost]
        public IActionResult PesquisarChamados(string id, string assunto, string data, string prioridade, string colaborador)
        {
            var chamados = _chamadoRepo.ListarTodosChamados();
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";

            var resultados = new List<Chamado>();

            // Percorre todos os chamados e filtra conforme os campos preenchidos
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

            // Retorna a View correspondente (técnico ou colaborador)
            if (papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return View("~/Views/DashboardTecnico/Index.cshtml", resultados);

            return View("~/Views/Home/Dashboard.cshtml", resultados);
        }

        // ===============================================================
        // AÇÕES DE GERENCIAMENTO DE USUÁRIOS (somente para técnicos)
        // ===============================================================

        // 🔹 Exibe lista de usuários cadastrados
        [HttpGet]
        public IActionResult Usuarios()
        {
            var papel = HttpContext.Session.GetString("Papel") ?? "colaborador";

            // Somente técnicos podem acessar esta área
            if (!papel.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            var usuarios = _usuarioRepo.ListarUsuarios();
            return View("~/Views/DashboardTecnico/Usuarios.cshtml", usuarios);
        }

        // 🔹 Cria um novo usuário (feito por técnico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarUsuarioPeloTecnico(string Nome, string Email, string Senha, string Papel)
        {
            var papelSess = HttpContext.Session.GetString("Papel") ?? "colaborador";

            // Bloqueia acesso se não for técnico
            if (!papelSess.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            // Valida campos obrigatórios
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                TempData["UsuarioMsgErro"] = "Nome, email e senha são obrigatórios.";
                return RedirectToAction("Usuarios");
            }

            // Cria objeto de usuário para inserção
            var novo = new Usuario
            {
                Nome = Nome.Trim(),
                Email = Email.Trim(),
                Senha = Senha,
                Papel = string.IsNullOrWhiteSpace(Papel) ? "colaborador" : Papel
            };

            // Tenta salvar no banco
            var ok = _usuarioRepo.CriarUsuario(novo);

            // Define mensagens de sucesso ou erro
            TempData[ok ? "UsuarioMsgSucesso" : "UsuarioMsgErro"] =
                ok ? "Usuário criado com sucesso." : "Falha ao criar usuário (verifique email duplicado).";

            return RedirectToAction("Usuarios");
        }

        // 🔹 Exclui um usuário (feito por técnico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirUsuario(int id)
        {
            var papelSess = HttpContext.Session.GetString("Papel") ?? "colaborador";
            var usuarioIdSess = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            // Permite apenas técnicos
            if (!papelSess.Equals("tecnico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            // Impede o técnico de excluir a própria conta
            if (id == usuarioIdSess)
            {
                TempData["UsuarioMsgErro"] = "Você não pode excluir sua própria conta enquanto estiver logado.";
                return RedirectToAction("Usuarios");
            }

            // Tenta excluir o usuário
            var ok = _usuarioRepo.ExcluirUsuario(id);

            TempData[ok ? "UsuarioMsgSucesso" : "UsuarioMsgErro"] =
                ok ? "Usuário excluído com sucesso." : "Falha ao excluir usuário.";

            return RedirectToAction("Usuarios");
        }
    }
}
