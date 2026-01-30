using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streetlight2._0.Data;
using Streetlight2._0.Models.LightModels;
using Streetlight2._0.Models.MaintenanceModels;

namespace Streetlight2._0.Services.LogServices
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LogService> _logger;

        public LogService(AppDbContext dbContext, ILogger<LogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> AddFaultyLightMaintenanceLog(int faultId, int userId, string action, string? remark, int changedSts)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                _logger.LogWarning("Invalid input: action is empty");
                return false;
            }

            try
            {
                var log = new FaultyLightMaintenanceLog
                {
                    FaultId = faultId,
                    UserId = userId,
                    Action = action,
                    Remark = remark,
                    ChangedStatus = changedSts
                };

                await _dbContext.FaultyLightMaintenanceLogs.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while saving Faulty Light Maintenance Log");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving Faulty Light Maintenance Log");
                return false;
            }
        }

        public async Task<bool> AddActionCommandLog(string commandFor, int userId, string action, string? remark)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                _logger.LogWarning("Invalid input: action is empty");
                return false;
            }

            try
            {
                var log = new LightActionLog
                {
                    CommandFor = commandFor,
                    UserId = userId,
                    Action = action,
                    ActionRemark = remark
                };

                await _dbContext.LightActionLogs.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while saving Faulty Light Maintenance Log");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving Faulty Light Maintenance Log");
                return false;
            }
        }
    }
}
