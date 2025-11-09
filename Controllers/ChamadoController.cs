using Microsoft.AspNetCore.Mvc; // Necessário para usar controladores e ações do ASP.NET MVC
using ProjetoTi.Data; // Importa a camada de acesso a dados (repositórios)
using ProjetoTi.Models; // Importa os modelos usados (neste caso, Chamado)
using System;
using System.Linq; // Necessário para usar LINQ (Count, Where, etc.)

namespace ProjetoTi.Controllers
{
    // Controlador responsável por gerenciar as operações de chamados (abrir, listar, visualizar, etc.)
    public class ChamadoController : Controller
    {
        // Repositório responsável pela comunicação com o banco de dados de chamados
        private readonly ChamadoRepository _chamRepo;

        // Construtor que recebe o repositório via injeção de dependência
        public ChamadoController(ChamadoRepository chamRepo)
        {
            _chamRepo = chamRepo;
        }

        // ===============================================================
        // AÇÃO: Dashboard
        // Exibe o painel principal do usuário com estatísticas e lista de chamados
        // ===============================================================
        [HttpGet]
        public IActionResult Dashboard()
        {
            // Recupera o ID do usuário logado da sessão
            var userId = HttpContext.Session.GetInt32("UsuarioId");

            // Se o usuário não estiver logado, redireciona para a página de Login
            if (userId == null)
                return RedirectToAction("Index", "Login");

            // Busca todos os chamados associados ao usuário logado
            var chamados = _chamRepo.ListarChamadosPorUsuario(userId.Value);

            // Envia informações adicionais para a View usando ViewBag
            ViewBag.NomeUsuario = HttpContext.Session.GetString("UsuarioNome"); // Nome do usuário logado
            ViewBag.TotalChamados = chamados.Count(); // Total de chamados
            ViewBag.Abertos = chamados.Count(c => c.Status == "aberto"); // Quantos estão abertos
            ViewBag.Fechados = chamados.Count(c => c.Status == "fechado"); // Quantos estão fechados

            // Retorna a View "Dashboard" com a lista de chamados como modelo
            return View(chamados);
        }

        // ===============================================================
        // AÇÃO: Meus
        // Exibe todos os chamados abertos pelo usuário logado
        // ===============================================================
        [HttpGet]
        public IActionResult Meus()
        {
            // Obtém o ID do usuário logado da sessão
            var userId = HttpContext.Session.GetInt32("UsuarioId");

            // Caso o usuário não esteja logado, redireciona para o login
            if (userId == null)
                return RedirectToAction("Index", "Login");

            // Busca apenas os chamados pertencentes a este usuário
            var meusChamados = _chamRepo.ListarChamadosPorUsuario(userId.Value);

            // Retorna a View "Meus" exibindo os chamados do usuário
            return View(meusChamados);
        }

        // ===============================================================
        // AÇÃO: Create (GET)
        // Exibe o formulário para abrir um novo chamado
        // ===============================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Retorna a View de criação de chamado
        }

        // ===============================================================
        // AÇÃO: Create (POST)
        // Recebe os dados enviados do formulário e cria um novo chamado no banco
        // ===============================================================
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra requisições forjadas (CSRF)
        public IActionResult Create(string Titulo, string Setor, string Prioridade, string Colaborador, string Descricao)
        {
            // Verifica se o usuário está logado
            var userIdObj = HttpContext.Session.GetInt32("UsuarioId");
            if (userIdObj == null)
            {
                // Se não estiver logado, exibe mensagem e redireciona
                TempData["MensagemErro"] = "Faça login antes de abrir um chamado.";
                return RedirectToAction("Index", "Login");
            }

            int userId = userIdObj.Value; // Converte o ID de usuário para inteiro

            // Monta a descrição completa do chamado (com setor, prioridade e colaborador)
            var fullDesc = $"Setor: {Setor}\nPrioridade: {Prioridade}\nColaborador: {Colaborador}\n\n{Descricao}";

            // Cria um novo objeto Chamado com os dados preenchidos
            var chamado = new Chamado
            {
                Titulo = Titulo?.Trim() ?? string.Empty, // Garante que o título não seja nulo
                Descricao = fullDesc, // Define a descrição montada acima
                Status = "aberto", // Define o status inicial do chamado
                IdUsuario = userId, // Vincula o chamado ao usuário logado
                DataAbertura = TimeZoneInfo.ConvertTimeFromUtc(
                     DateTime.UtcNow, // Pega a hora atual em UTC
                     TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time") // Converte para horário de Brasília
                )
            };

            try
            {
                // Chama o repositório para salvar o chamado no banco (Supabase)
                var ok = _chamRepo.CriarChamado(chamado);

                // Se o método retornar verdadeiro, o chamado foi criado com sucesso
                if (ok)
                {
                    // Redireciona para a tela de sucesso
                    return RedirectToAction("Success");
                }
                else
                {
                    // Se não deu certo, mostra uma mensagem de erro na própria tela
                    ViewBag.MensagemErro = "Não foi possível abrir o chamado. Tente novamente.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Captura erros inesperados (ex: falha de conexão ou erro SQL)
                ViewBag.MensagemErro = "Erro: " + ex.Message;
                return View(); // Retorna a View de criação exibindo a mensagem
            }
        }

        // ===============================================================
        // AÇÃO: Success
        // Exibe uma tela simples de confirmação após abrir um chamado
        // ===============================================================
        [HttpGet]
        public IActionResult Success()
        {
            return View(); // Retorna a View "Success"
        }
    }
}
