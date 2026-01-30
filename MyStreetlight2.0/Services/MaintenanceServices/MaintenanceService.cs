using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Streetlight2._0.Data;
using Streetlight2._0.DTOs.MaintenanceDtos;
using Streetlight2._0.Services.CommonDataService;
using Streetlight2._0.Services.UserService;

namespace Streetlight2._0.Services.MaintenanceServices
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<MaintenanceService> _logger;
        private readonly ICommonDataService _commonDataService;
        private readonly IUserService _userService;

        public MaintenanceService(
            AppDbContext dbContext,
            ILogger<MaintenanceService> logger,
            ICommonDataService commonDataService,
            IUserService userService
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _commonDataService = commonDataService;
            _userService = userService;
        }

        public async Task<List<FaultyLightDto>> GetAllFaultyLights()
        {
            try
            {
                var faultyLights = await (
                    from log in _dbContext.FaultyLightLogs
                    join light in _dbContext.LightsMasters
                        on new { log.GatewayId, log.NodeId }
                        equals new { light.GatewayId, light.NodeId }
                    select new FaultyLightDto
                    {
                        FaultId = log.RecordId,
                        FaultAt = log.CreatedAt,
                        LightId = light.LightId,
                        GatewayId = light.GatewayId,
                        NodeId = light.NodeId ?? 0,
                        Address = light.Address,
                        Latitude = light.Latitude,
                        Longitude = light.Longitude,
                        FaultType = log.LightStatus,
                        Status = log.Status
                    })
                    .ToListAsync();

                if (faultyLights == null || !faultyLights.Any())
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(faultyLights));
                    return null;
                }

                return faultyLights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching Faulty Light Data");

                throw;
            }
        }

        public async Task<List<FaultyLightDto>> GetAllFaultyLights(int? status)
        {
            try
            {
                var faultyLightsQuery =
                    from log in _dbContext.FaultyLightLogs
                    join light in _dbContext.LightsMasters
                        on new { log.GatewayId, log.NodeId }
                        equals new { light.GatewayId, light.NodeId }
                    select new { log, light };

                //Console.WriteLine(JsonConvert.SerializeObject(faultyLightsQuery));

                if (faultyLightsQuery == null || !faultyLightsQuery.Any())
                {
                    return null;
                }

                faultyLightsQuery = faultyLightsQuery.Where(x => x.log.Status == status);

                var faultyLights = await faultyLightsQuery
                    .Select(x => new FaultyLightDto
                    {
                        FaultId = x.log.RecordId,
                        FaultAt = x.log.CreatedAt,
                        LightId = x.light.LightId,
                        GatewayId = x.light.GatewayId,
                        NodeId = x.light.NodeId ?? 0,
                        Address = x.light.Address,
                        Latitude = x.light.Latitude,
                        Longitude = x.light.Longitude,
                        FaultType = x.log.LightStatus,
                        Status = x.log.Status
                    })
                    .ToListAsync();

                //Console.WriteLine(JsonConvert.SerializeObject(faultyLights));
                if (faultyLights == null || !faultyLights.Any())
                {
                    return null;
                }

                return faultyLights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching Faulty Light Data");

                throw;
            }
        }

        public async Task<bool> UpdateFaultyLightStatus(int faultId, int newStatus)
        {
            try
            {
                var faultyLightLog = await _dbContext.FaultyLightLogs.FirstOrDefaultAsync(f => f.RecordId == faultId);

                if (faultyLightLog == null)
                {
                    _logger.LogWarning($"Faulty Light Log with ID {faultId} not found");
                    return false;
                }

                faultyLightLog.Status = newStatus;

                _dbContext.FaultyLightLogs.Update(faultyLightLog);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating Faulty Light status");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating Faulty Light status");
                return false;
            }
        }

        public async Task<FaultyLightMaintenanceLogDataDto> GetFaultyLightMaintenanceLogsAsync(int faultId)
        {
            try
            {
                var faultyLightLog = await _dbContext.FaultyLightLogs
                    .FirstOrDefaultAsync(f => f.RecordId == faultId);

                if (faultyLightLog == null)
                {
                    _logger.LogWarning($"Faulty Light Log with ID {faultId} not found");

                    return null;
                }

                Console.WriteLine(JsonConvert.SerializeObject(faultyLightLog));

                var maintenanceLogs = await _dbContext.FaultyLightWithMaintenanceLogs
                    .Where(m => m.FaultId == faultId)
                    .Select(m => new MaintenanceLog
                    {
                        UserName = m.UserId.ToString(),
                        Action = m.Action,
                        Remark = m.Remark,
                        LoggedAt = m.LoggedAt
                    })
                    .ToListAsync();

                Console.WriteLine(JsonConvert.SerializeObject(maintenanceLogs));

                var result = new FaultyLightMaintenanceLogDataDto
                {
                    FaultId = faultyLightLog.RecordId,
                    GatewayId = faultyLightLog.GatewayId,
                    NodeId = faultyLightLog.NodeId,
                    FaultyStatus = faultyLightLog.LightStatus,
                    FaultedAt = faultyLightLog.CreatedAt,
                    MaintenanceStatus = faultyLightLog.Status,
                    MaintenanceLogs = maintenanceLogs
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching Maintenance Logs for Fault ID {faultId}");

                throw;
            }
        }
    }
}
