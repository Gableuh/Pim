using ProjetoTi.Data;              // Acesso ao repositório de usuários
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// 🔹 Serviços do sistema
// ========================================

// Adiciona suporte a Controllers e Views (MVC)
builder.Services.AddControllersWithViews();

// Configura a sessão (para login e autenticação)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // expira após 30 minutos de inatividade
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registra o repositório de usuários (injeção de dependência)
builder.Services.AddScoped<UsuarioRepository>();

var app = builder.Build();

// ========================================
// 🔹 Inicialização do banco e usuário de teste
// ========================================
using (var scope = app.Services.CreateScope())
{
    var usuarioRepo = scope.ServiceProvider.GetRequiredService<UsuarioRepository>();

    try
    {
        usuarioRepo.CriarUsuarioTeste(); // cria usuário de teste se não existir
        Console.WriteLine("✅ Usuário de teste garantido: gabriel@teste.com / 123456");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Erro ao inicializar usuário de teste: {ex.Message}");
    }
}

// ========================================
// 🔹 Configuração do pipeline HTTP
// ========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // força HTTPS
app.UseStaticFiles();      // habilita CSS, JS, imagens
app.UseRouting();          // ativa o roteamento MVC
app.UseSession();          // ativa o uso de sessão
app.UseAuthorization();    // ativa autenticação/autorização (futuro)

// ========================================
// 🔹 Definição de rotas
// ========================================

// Rota padrão (abrirá Login por padrão)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

// Endpoint opcional de teste da conexão com o banco
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
