//Esse controlador é simples, mas essencial — ele permite que o técnico visualize todos os chamados e altere o status de qualquer um para “Fechado”.

using Microsoft.AspNetCore.Mvc; // Necessário para usar controladores e ações no ASP.NET MVC
using ProjetoTi.Data; // Importa os repositórios e classes de acesso ao banco de dados
using ProjetoTi.Models; // Importa os modelos utilizados no sistema (ex: Chamado)

namespace ProjetoTi.Controllers
{
    // Controlador responsável pela dashboard (painel) destinada aos Técnicos de TI
    // Aqui o técnico pode visualizar e gerenciar todos os chamados do sistema
    public class DashboardTecnicoController : Controller
    {
        // Instância do repositório de chamados, responsável pelas operações com o banco (Supabase)
        private readonly ChamadoRepository _chamadoRepo = new ChamadoRepository();

        // ===============================================================
        // AÇÃO: Index (GET)
        // Exibe a página principal do painel do técnico com todos os chamados
        // ===============================================================
        public IActionResult Index()
        {
            // Busca todos os chamados do banco de dados para exibição ao técnico
            var chamados = _chamadoRepo.ListarTodosChamados();

            // Recupera o nome do usuário logado armazenado na sessão
            // Se não houver nome na sessão, exibe "Técnico" por padrão
            ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome") ?? "Técnico";

            // Retorna a View "Index" com a lista de chamados como modelo
            return View(chamados);
        }

        // ===============================================================
        // AÇÃO: Fechar (POST)
        // Atualiza o status de um chamado para "Fechado"
        // ===============================================================
        [HttpPost]
        public IActionResult Fechar(int id)
        {
            // Chama o repositório para atualizar o status do chamado com o ID informado
            _chamadoRepo.AtualizarStatus(id, "Fechado");

            // Após fechar o chamado, redireciona novamente para o painel principal
            return RedirectToAction("Index");
        }
    }
}
