using QA.Common.Services;
using QA.Data;

namespace DbDataToJson;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(IHostApplicationLifetime hostApplicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var appender = scope.ServiceProvider.GetRequiredService<DumpService>();
        await appender.Dump();
        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}