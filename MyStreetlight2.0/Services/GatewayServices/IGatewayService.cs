using Streetlight2._0.DTOs.GatewayDtos;

namespace Streetlight2._0.Services.GatewayService
{
    public interface IGatewayService
    {
        Task<List<GatewayDto>> GetAllInstalledGateways();

        Task<GatewayDto> GetGatewayDataByIdAsync(string gatewayId);
    }
}
