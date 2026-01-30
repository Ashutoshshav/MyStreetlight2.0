using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Streetlight2._0.Data;
using Streetlight2._0.DTOs.LightDtos;
using Streetlight2._0.Models;
using Streetlight2._0.Models.GatewayModels;
using Streetlight2._0.Models.LightModels;
using Streetlight2._0.Services.CommonDataService;
using Streetlight2._0.Services.StreetlightService;

namespace Streetlight2._0.Services.StreetlightService
{
    public class StreetlightService : IStreetlightService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StreetlightService> _logger;
        private readonly ICommonDataService _commonDataService;

        public StreetlightService(
            AppDbContext dbContext,
            ILogger<StreetlightService> logger,
            ICommonDataService commonDataService
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _commonDataService = commonDataService;
        }

        public async Task<string> GetStreetlightStatus(string macId)
        {
            try
            {
                var light = _dbContext.LightLiveData.FirstOrDefault(l => l.MacId == macId);

                if (light == null)
                {
                    return "Light Data not found";
                }

                string lightStatus = await _commonDataService.GetLightStsNameByStsId(light.LightStatus ?? -1);

                return lightStatus;
            }
            catch (Exception)
            {
                return "Error getting light live data";
            }
        }

        public async Task<LightLiveDataDto> GetLightLiveDataByMacId(string macId)
        {
            try
            {
                var light = await _dbContext.LightLiveData.FirstOrDefaultAsync(l => l.MacId == macId);

                //Console.WriteLine(JsonConvert.SerializeObject(light));
                //Console.WriteLine(macId);

                if (light == null)
                {
                    return null;
                }

                var lightSts = await _commonDataService.GetLightStsNameByStsId(light.LightStatus ?? -1);

                return new LightLiveDataDto
                {
                    RecordId = light.RecordId,
                    UpdatedAt = light.UpdatedAt,
                    LightId = light.LightId,
                    MacId = light.MacId,
                    LightStatus = lightSts,
                    Ampere = light.Ampere
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LightLiveDataDto> GetLightLiveDataByLightId(string lightId)
        {
            try
            {
                var light = await _dbContext.LightLiveData.FirstOrDefaultAsync(l => l.LightId == lightId);

                if (light == null)
                {
                    return null;
                }

                var lightSts = await _commonDataService.GetLightStsNameByStsId(light.LightStatus ?? -1);
                return new LightLiveDataDto
                {
                    RecordId = light.RecordId,
                    UpdatedAt = light.UpdatedAt,
                    LightId = light.LightId,
                    MacId = light.MacId,
                    LightStatus = lightSts,
                    Ampere = light.Ampere
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<decimal> GetCumulativeONMinByMacId(string macId)
        {
            try
            {
                var lightLiveDataLog = await _dbContext.LightLiveDataLogs
                    .Where(l => l.MacId == macId)
                    .OrderByDescending(l => l.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lightLiveDataLog == null)
                {
                    return 0;
                }

                return lightLiveDataLog.Comin ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching CumulativeONMin for MacId: {macId}");

                throw;
            }
        }

        public async Task<LightLiveDataDto> GetLightLiveDataByGatewayIdAndNodeId(string gatewayId, int nodeId)
        {
            try
            {
                var light = await _dbContext.LightLiveData.FirstOrDefaultAsync(l => l.GatewayId == gatewayId && l.NodeId == nodeId);

                //Console.WriteLine(JsonConvert.SerializeObject(light));
                //Console.WriteLine(gatewayId + " " + nodeId);

                if (light == null)
                {
                    return null;
                }

                var lightSts = await _commonDataService.GetLightStsNameByStsId(light.LightStatus ?? -1);

                return new LightLiveDataDto
                {
                    RecordId = light.RecordId,
                    UpdatedAt = light.UpdatedAt,
                    LightId = light.LightId,
                    MacId = light.MacId,
                    LightStatus = lightSts,
                    Ampere = light.Ampere
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<decimal> GetCumulativeONMinByGatewayIdAndNodeId(string gatewayId, int nodeId)
        {
            try
            {
                var lightLiveDataLog = await _dbContext.LightLiveDataLogs
                    .Where(l => l.GatewayId == gatewayId && l.NodeId == nodeId)
                    .OrderByDescending(l => l.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lightLiveDataLog == null)
                {
                    return 0;
                }

                return lightLiveDataLog.Comin ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching CumulativeONMin for GatewayId {gatewayId} and NodeId {nodeId}");

                throw;
            }
        }

        public async Task<List<LightWithLiveData>> GetAllLightsWithLiveDataAsync()
        {
            try
            {
                var data = await _dbContext.LightWithLiveData.ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching All Lights With its Live Data");

                throw;
            }
        }
    }
}
