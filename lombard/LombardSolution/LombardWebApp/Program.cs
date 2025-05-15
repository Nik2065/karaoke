using Microsoft.AspNetCore.Hosting;

namespace LombardWebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
            var host = CreateHostBuilder(args).Build();

            // Инициализация базы данных
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }


            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
            .UseStartup<Startup>();

        });
    }
}