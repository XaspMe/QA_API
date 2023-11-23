using DbDataToJson;
using Microsoft.EntityFrameworkCore;
using QA.Common.Services;
using QA.Data;

var dBConnectionString = Environment.GetEnvironmentVariable("QA_DB", EnvironmentVariableTarget.Machine);
if (dBConnectionString is "" or null)
    throw new ArgumentException("QA_DB environment variable does not exists on this machine or empty");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<QaContext>(opt =>
            opt.UseSqlServer(dBConnectionString));
        services.AddScoped<IQaRepo, SqlQaRepo>();
        services.AddScoped<DumpService>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
