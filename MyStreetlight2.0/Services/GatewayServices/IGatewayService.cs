using MyStreetlight2._0.DTOs.GatewayDtos;

namespace MyStreetlight2._0.Services.GatewayService
{
    public interface IGatewayService
    {
        Task<List<GatewayDto>> GetAllInstalledGateways();

        Task<GatewayDto> GetGatewayDataByIdAsync(string gatewayId);
    }
}
