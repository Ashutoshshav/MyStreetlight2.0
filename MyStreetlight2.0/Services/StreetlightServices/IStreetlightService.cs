using Streetlight2._0.DTOs.LightDtos;
using Streetlight2._0.Models;
using Streetlight2._0.Models.LightModels;

namespace Streetlight2._0.Services.StreetlightService
{
    public interface IStreetlightService
    {
        Task<string> GetStreetlightStatus(string macId);

        Task<LightLiveDataDto> GetLightLiveDataByMacId(string macId);

        Task<LightLiveDataDto> GetLightLiveDataByGatewayIdAndNodeId(string gatewayId, int nodeId);

        Task<LightLiveDataDto> GetLightLiveDataByLightId(string lightId);

        Task<decimal> GetCumulativeONMinByMacId(string macId);

        Task<decimal> GetCumulativeONMinByGatewayIdAndNodeId(string gatewayId, int nodeId);

        Task<List<LightWithLiveData>> GetAllLightsWithLiveDataAsync();
    }
}
