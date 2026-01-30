using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Streetlight2._0.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

public class LightUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LightUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Update all LightsMasters -> Response = -1000
                var allLights = dbContext.LightsMasters.ToList();
                foreach (var light in allLights)
                {
                    light.Response = -1000;
                }

                await dbContext.SaveChangesAsync();
            }

            // Wait for 5 minutes before next run
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
