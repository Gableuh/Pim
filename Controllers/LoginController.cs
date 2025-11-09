using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoTi.Data;
using ProjetoTi.Models;

namespace ProjetoTi.Controllers
{
    public class LoginController : Controller
    {
        // Instância do repositório responsável por acessar os dados dos usuários
        private readonly UsuarioRepository _repo = new UsuarioRepository();

        // ================================================================
        // GET: /Login
        // ================================================================
        [HttpGet]
        public IActionResult Index()
        {
            // Retorna a View de login (tela onde o usuário insere email, senha e papel)
            return View();
        }

        // ================================================================
        // POST: /Login
        // ================================================================
        [HttpPost]
        public IActionResult Index(string email, string password, string role)
        {
            // Armazena temporariamente os valores digitados para reaproveitar na tela
            // caso o login falhe (assim o usuário não precisa redigitar tudo)
            ViewBag.Email = email;
            ViewBag.Role = role;

            // 🔹 Validação básica: impede login com campos vazios
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.MensagemErro = "Todos os campos devem estar preenchidos.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }

            // 🔹 Autenticação: busca o usuário no banco (Supabase) com base no email e senha
            var usuario = _repo.Autenticar(email, password);

            // Se não encontrou o usuário, exibe mensagem de erro
            if (usuario == null)
            {
                ViewBag.MensagemErro = "Usuário ou senha inválidos.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }

            // 🔹 Verifica se o papel (colaborador/técnico/gestor) selecionado no login
            // coincide com o papel do usuário cadastrado no banco
            if (!string.Equals(usuario.Papel, role, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.MensagemErro = "Email cadastrado já tem um login diferente.";
                ViewBag.Email = email;
                ViewBag.Role = role;
                return View();
            }

            // ================================================================
            // LOGIN BEM-SUCEDIDO
            // ================================================================

            // Armazena as informações principais do usuário na sessão
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);     // ID do usuário
            HttpContext.Session.SetString("Papel", usuario.Papel);     // Função (colaborador/técnico)
            HttpContext.Session.SetString("NomeUsuario", usuario.Nome); // Nome exibido na dashboard

            // Redireciona o usuário autenticado para a Home (dashboard)
            return RedirectToAction("Index", "Home");
        }

        // ================================================================
        // GET: /Logout
        // ================================================================
        public IActionResult Logout()
        {
            // 🔹 Limpa todas as informações da sessão (encerra a autenticação)
            HttpContext.Session.Clear();

            // Redireciona de volta para a tela de login
            return RedirectToAction("Index", "Login");
        }
    }
}
