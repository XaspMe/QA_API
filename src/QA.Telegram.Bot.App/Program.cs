using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA.Common.Data;
using QA.Telegram.Bot.App;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<QaContext>(opt =>
            opt.UseSqlServer(
                "Server=localhost;initial catalog=QA_DB;user ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=True"));
        services.AddHostedService<BotService>();
        services.AddScoped<IQaRepo, SqlQaRepo>();
    })
    .Build();

host.Run();