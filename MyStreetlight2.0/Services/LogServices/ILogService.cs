namespace Streetlight2._0.Services.LogServices
{
    public interface ILogService
    {
        Task<bool> AddFaultyLightMaintenanceLog(int faultId, int userId, string action, string remark, int changedSts);
    }
}
