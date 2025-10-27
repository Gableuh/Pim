using System;
using Npgsql;

namespace ProjetoTi.Data
{
    public class DatabaseTest
    {
        public static void TestConnection()
        {
            var connectionString = "Host=db.lfvhvtbnnwpqyjzaaovi.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ProjetoTi123;SSL Mode=Require;Trust Server Certificate=true;";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("✅ Conexão com o Supabase estabelecida com sucesso!");
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Falha ao conectar ao Supabase:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
