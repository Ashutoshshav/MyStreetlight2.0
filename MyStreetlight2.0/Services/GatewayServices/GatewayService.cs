using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs.GatewayDtos;
using MyStreetlight2._0.Services.CommonDataService;

namespace MyStreetlight2._0.Services.GatewayService
{
    public class GatewayService : IGatewayService
    {
        private readonly AppDbContext _dbContext;
        private readonly ICommonDataService _commonDataService;
        private readonly ILogger<GatewayService> _logger;

        public GatewayService(
            AppDbContext dbContext,
            ICommonDataService commonDataService,
            ILogger<GatewayService> logger
        )
        {
            _dbContext = dbContext;
            _commonDataService = commonDataService;
            _logger = logger;
        }

        //public async Task<List<GatewayDto>> GetAllInstalledGateways()
        //{
        //    var gateways = await _dbContext.LightsMasters
        //        .Where(l => !string.IsNullOrEmpty(l.GatewayId))
        //        .Select(l => new 
        //        {
        //            l.GatewayId,
        //            l.MacId
        //        })
        //        .ToListAsync();

        //    var lightStatus = await _dbContext.LightLiveData
        //        .Where(l => l.GatewayId == gateways.First().GatewayId)
        //        .ToArrayAsync();

        //    List<GatewayDto> lightWithLiveData = new List<GatewayDto>();
        //    var distinctGateways = gateways
        //        .GroupBy(g => g.GatewayId)
        //        .Select(g => g.Key)
        //        .ToList();

        //    Console.WriteLine(JsonConvert.SerializeObject(distinctGateways));
        //    foreach (var gateway in distinctGateways)
        //    {
        //        var lightLiveData = lightStatus.FirstOrDefault(l => l.GatewayId == gateway);

        //        var newLightLiveData = new GatewayDto
        //        {
        //            Id = gateway,
        //            LightCount = gateways.Count(x => x.GatewayId == gateway),
        //            OnLightCount = lightStatus.Count(x => x.LightStatus == 1 && x.GatewayId == gateway),
        //            OffLightCount = lightStatus.Count(x => x.LightStatus == 0 && x.GatewayId == gateway),
        //            NGLightCount = lightStatus.Count(x => x.LightStatus == 2 && x.GatewayId == gateway),
        //            NCLightCount = lightStatus.Count(x => x.LightStatus == 3 && x.GatewayId == gateway),
        //        };

        //        lightWithLiveData.Add(newLightLiveData);
        //    }

        //    Console.WriteLine(JsonConvert.SerializeObject(lightWithLiveData));

        //    //var gatewayDtos = gateways
        //    //    .GroupBy(g => g.GatewayId)
        //    //    .Select(g => new GatewayDto
        //    //    {
        //    //        Id = g.Key,
        //    //        LightCount = gateways.Count(x => x.GatewayId == g.Key)
        //    //    })
        //    //    .ToList();

        //    return lightWithLiveData;
        //}

        public async Task<List<GatewayDto>> GetAllInstalledGateways()
        {
            try
            {
                var lightWithLiveData = await _dbContext.LightWithLiveData.ToListAsync();
                var threshold = DateTime.Now.AddMinutes(-30);

                var gatewayDtos = lightWithLiveData
                    .GroupBy(g => g.GatewayId)
                    .Select(g => new GatewayDto
                    {
                        Id = g.Key,
                        LightCount = lightWithLiveData.Count(x => x.GatewayId == g.Key),
                        OnLightCount = lightWithLiveData.Count(x => (x.LightStatus == 1 || x.LightStatus == 2) && x.UpdatedAt > threshold && x.GatewayId == g.Key),
                        OffLightCount = lightWithLiveData.Count(x => x.LightStatus == 0 && x.UpdatedAt > threshold && x.GatewayId == g.Key),
                        NGLightCount = 0,
                        NCLightCount = lightWithLiveData.Count(x => (x.LightStatus == 3 || x.UpdatedAt < threshold) && x.GatewayId == g.Key)
                    })
                    .ToList();

                return gatewayDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching All Installed Gateways");
                return new List<GatewayDto>();
            }
        }

        public async Task<GatewayDto?> GetGatewayDataByIdAsync(string gatewayId)
        {
            try
            {
                var gatewayData = await _dbContext.Gateways
                    .Where(g => g.GatewayId == gatewayId)
                    .Select(g => new GatewayDto
                    {
                        Id = g.GatewayId,
                        Zone = g.Zone,
                        Ward = g.Ward,
                        Address = g.Address,
                        Latitude = g.Latitude,
                        Longitude = g.Longitude,
                        UpdatedAt = g.UpdatedAt,
                        RadioStatus = g.RadioStatus,
                        LightCount = g.ConnectedLights
                    })
                    .SingleOrDefaultAsync();

                return gatewayData; // returns null if not found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Gateway with ID {GatewayId}", gatewayId);
                throw; // Rethrow so higher layers (Controller/API) can decide the response
            }
        }
    }
}
