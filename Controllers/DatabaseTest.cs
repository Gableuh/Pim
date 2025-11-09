//Este arquivo serve como um teste de verificação de conexão com o banco do Supabase (PostgreSQL) — ideal para garantir que a string de conexão está correta e que o servidor está acessível antes de usar em produção.

using System; // Necessário para funcionalidades básicas do .NET (como Console e Exception)
using Npgsql; // Biblioteca para conexão com bancos de dados PostgreSQL (usada pelo Supabase)

namespace ProjetoTi.Data
{
    // Classe utilizada para testar a conexão com o banco de dados Supabase (PostgreSQL)
    public class DatabaseTest
    {
        // ===============================================================
        // Método: TestConnection()
        // Objetivo: Testar se a aplicação consegue se conectar ao banco Supabase
        // ===============================================================
        public static void TestConnection()
        {
            // String de conexão contendo as credenciais e informações de acesso ao banco
            var connectionString =
                "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;" +
                "Port=5432;" +
                "Database=postgres;" +
                "Username=postgres;" +
                "Password=ProjetoTi123;" +
                "SSL Mode=Require;" + // Requer conexão segura (SSL)
                "Trust Server Certificate=true;"; // Ignora validação do certificado SSL

            try
            {
                // Cria uma nova conexão com o banco de dados usando a string acima
                using var conn = new NpgsqlConnection(connectionString);

                // Tenta abrir a conexão com o Supabase
                conn.Open();

                // Se chegar aqui, significa que a conexão foi bem-sucedida
                Console.WriteLine("✅ Conexão com o Supabase estabelecida com sucesso!");

                // Fecha a conexão explicitamente (boa prática)
                conn.Close();
            }
            catch (Exception ex)
            {
                // Caso ocorra qualquer erro na tentativa de conexão, exibe mensagem de falha
                Console.WriteLine("❌ Falha ao conectar ao Supabase:");
                Console.WriteLine(ex.Message); // Mostra detalhes do erro no console
            }
        }
    }
}
