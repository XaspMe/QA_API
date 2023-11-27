using MediatR;
using Microsoft.EntityFrameworkCore;
using QA.Data;
using QA.Telegram.Bot.App;
using QA.Telegram.Bot.App.Feature;

var dBConnectionString = Environment.GetEnvironmentVariable("QA_DB", EnvironmentVariableTarget.User);
if (dBConnectionString is "" or null)
{
    dBConnectionString =
        Environment.GetEnvironmentVariable("QA_DB", EnvironmentVariableTarget.Process);
    if (dBConnectionString is "" or null)
    {
        throw new ArgumentException("QA_DB environment variable dos not exists on this machine or empty");
    }
}


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<QaContext>(opt =>
            opt.UseSqlServer(dBConnectionString));
        services.AddHostedService<BotService>();
        services.AddScoped<IQaRepo, SqlQaRepo>();
        services.AddMemoryCache();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UserAdminAccessBehavior<,>));
    })
    .Build();

host.Run();