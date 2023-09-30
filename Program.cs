using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace QA_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                    //     webBuilder.UseUrls("http://62.113.97.22:5000", "https://62.113.97.22:5001");
                });
    }
}