using Microsoft.EntityFrameworkCore;
using QA.Data;
using QA.Telegram.Bot.App;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<QaContext>(opt =>
            opt.UseSqlServer(
                //todo move to config
                "Server=localhost;initial catalog=QA_DB;user ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=True"));
        services.AddHostedService<BotService>();
        services.AddScoped<IQaRepo, SqlQaRepo>();
        services.AddMemoryCache();
    })
    .Build();

host.Run();