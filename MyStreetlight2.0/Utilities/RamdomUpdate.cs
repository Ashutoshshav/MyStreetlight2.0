using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs.MicsDtos;
using static System.Formats.Asn1.AsnWriter;

namespace MyStreetlight2._0.Utilities
{
    public class RamdomUpdate : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LightUpdateFlag _flag;

        public RamdomUpdate(IServiceProvider serviceProvider, LightUpdateFlag flag)
        {
            _serviceProvider = serviceProvider;
            _flag = flag;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var random = new Random();

                    var allData = await _dbContext.LightsMasters
                        .Where(x => x.Response != null && x.Response != -1000)
                        .Select(x => new
                        {
                            x.GatewayId,
                            NodeId = x.NodeId ?? 0,
                            x.Response
                        })
                        .ToListAsync();

                    var newData = await _dbContext.LightLiveData.ToListAsync();
                    var lightData = await _dbContext.LightsMasters.ToListAsync();

                    var selectedLights = allData.OrderBy(x => random.Next()).Take(15).ToList();

                    foreach (var item in selectedLights)
                    {
                        var liveData = newData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);
                        var liveData2 = lightData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);

                        if (liveData != null && liveData2 != null)
                        {
                            liveData.UpdatedAt = DateTime.Now;
                            liveData.LightStatus = (item.Response == 2 || item.Response == -1000)
                                ? liveData.LightStatus
                                : item.Response;

                            if (item.Response == 2 || item.Response == -1000)
                            {
                                continue;
                            }
                            else if (item.Response == 1)
                            {
                                liveData.Ampere = Math.Round((decimal)(random.NextDouble() * (0.4 - 0.3) + 0.4), 2);
                            }
                            else if (item.Response == 0)
                            {
                                liveData.Ampere = 0;
                            }

                            //liveData2.Response = -1000;
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }

                // Delay based on flag
                var delay = _flag.IsHigh ? 25 : 15;
                var count = _flag.TaskCount;

                //Console.WriteLine(delay);
                //Console.WriteLine(count);

                await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);

                _flag.IsHigh = false;
            }
        }
    }
}
