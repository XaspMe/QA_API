using Microsoft.EntityFrameworkCore;
using QA.Data;
using QA.Telegram.Bot.App;

var dBConnectionString = Environment.GetEnvironmentVariable("QA_DB", EnvironmentVariableTarget.User);
if (dBConnectionString is "" or null)
{
    throw new NotImplementedException("QA_DB environment variable does not exists on this machine or empty");
}

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<QaContext>(opt =>
            opt.UseSqlServer(dBConnectionString));
        services.AddHostedService<BotService>();
        services.AddScoped<IQaRepo, SqlQaRepo>();
        services.AddMemoryCache();
    })
    .Build();

host.Run();