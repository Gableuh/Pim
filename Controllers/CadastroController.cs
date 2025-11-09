using Microsoft.AspNetCore.Mvc; // Necessário para usar controladores e ações MVC
using ProjetoTi.Data; // Importa o namespace onde está o repositório de usuários
using ProjetoTi.Models; // Importa o modelo "Usuario"

namespace ProjetoTi.Controllers
{
    // Controlador responsável pelo cadastro de novos usuários no sistema
    public class CadastroController : Controller
    {
        // Repositório para acessar o banco de dados de usuários
        private readonly UsuarioRepository _usuarioRepo;

        // Construtor da classe — inicializa o repositório
        public CadastroController()
        {
            _usuarioRepo = new UsuarioRepository(); // Cria uma instância do repositório de usuários
        }

        // -----------------------------
        // AÇÃO GET: Exibe o formulário de cadastro
        // -----------------------------
        [HttpGet]
        public IActionResult Index()
        {
            // Retorna a View "Index" dentro da pasta "Views/Cadastro"
            return View();
        }

        // -----------------------------
        // AÇÃO POST: Recebe e processa os dados enviados pelo formulário de cadastro
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF (Cross-Site Request Forgery)
        public IActionResult Index(Usuario model, string confirmPassword)
        {
            // Verifica se os dados do formulário são válidos conforme as validações do modelo (ModelState)
            if (!ModelState.IsValid)
                return View(model); // Retorna a view com os erros de validação

            // Valida se a senha foi informada e se a confirmação coincide
            if (string.IsNullOrWhiteSpace(model.Senha) || model.Senha != confirmPassword)
            {
                // Adiciona um erro personalizado ao ModelState
                ModelState.AddModelError(string.Empty, "As senhas não coincidem ou estão vazias.");
                return View(model); // Retorna a view mostrando a mensagem de erro
            }

            try
            {  

                // Chama o método do repositório que insere o novo usuário no banco de dados (Supabase)
                _usuarioRepo.CriarUsuario(model);

                // Armazena uma mensagem temporária de sucesso que será exibida na próxima requisição
                TempData["Success"] = "Cadastro realizado com sucesso! Faça login para continuar.";

                // Redireciona o usuário para a página de Login após o cadastro bem-sucedido
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro (ex: problema de conexão ou inserção no banco), exibe a mensagem de erro
                ModelState.AddModelError(string.Empty, "Erro ao cadastrar usuário: " + ex.Message);
                return View(model); // Retorna a mesma view com o erro
            }
        }
    }
}
