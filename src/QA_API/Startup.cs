using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using QA.Common.Services;
using QA.Data;

namespace QA_API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDbContext<QaContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("QA_Connection")))
            .AddControllers();
        services
            .AddMvcCore();
        services.AddSwaggerGen();
        services.AddScoped<IQaRepo, SqlQaRepo>();
        services.AddAutoMapper(typeof(Startup));
        services.AddScoped<DumpService>();
        services.UseHttpClientMetrics();
        services.AddHealthChecks()
            .AddCheck<HealthCheck>(nameof(HealthCheck))
            .ForwardToPrometheus();
        services.AddHealthChecks();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseCors("AllowAll");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}

public sealed class HealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}