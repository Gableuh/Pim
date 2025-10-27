using ProjetoTi.Data; // Importa o repositório
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Adiciona serviços MVC (controllers + views)
builder.Services.AddControllersWithViews();

// 🔹 Adiciona gerenciamento de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // expira após 30 minutos de inatividade
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 Registra o repositório de usuários para injeção de dependência
builder.Services.AddScoped<UsuarioRepository>();

var app = builder.Build();

// 🔹 Garante que o banco está acessível e cria o usuário de teste
using (var scope = app.Services.CreateScope())
{
    var usuarioRepo = scope.ServiceProvider.GetRequiredService<UsuarioRepository>();
    usuarioRepo.CriarUsuarioTeste(); // cria se não existir
    Console.WriteLine("✅ Usuário de teste garantido: gabriel@teste.com / 123456");
}

// 🔹 Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // força HTTPS
app.UseStaticFiles();      // permite carregar CSS, JS, imagens

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 🔹 Rota padrão (vai abrir a tela de Login ao iniciar)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

// 🔹 Endpoint opcional de teste do banco (apenas para debug)
app.MapGet("/test-db", (UsuarioRepository repo) =>
{
    try
    {
        var usuario = repo.Autenticar("gabriel@teste.com", "123456");
        return usuario != null
            ? Results.Ok("✅ Conexão bem-sucedida com o banco e usuário encontrado!")
            : Results.Problem("⚠️ Conexão OK, mas usuário não encontrado.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"❌ Erro ao conectar: {ex.Message}");
    }
});

app.Run();
