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
using QA.Common.Data;
using QA.Common.Services;

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
            // Define a sample health check that always signals healthy state.
            .AddCheck<SampleHealthCheck>(nameof(SampleHealthCheck))
            // Report health check results in the metrics output.
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

    // todo move to db
    private void InitializeDatabase(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<QaContext>().Database.Migrate();
        }
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
        InitializeDatabase(app);

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

public sealed class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // todo move to service folder
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}