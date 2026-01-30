using MyStreetlight2._0.DTOs.MaintenanceDtos;

namespace MyStreetlight2._0.Services.MaintenanceServices
{
    public interface IMaintenanceService
    {
        Task<List<FaultyLightDto>> GetAllFaultyLights();

        Task<List<FaultyLightDto>> GetAllFaultyLights(int? status);

        Task<bool> UpdateFaultyLightStatus(int faultId, int newStatus);

        Task<FaultyLightMaintenanceLogDataDto> GetFaultyLightMaintenanceLogsAsync(int faultId);
    }
}
