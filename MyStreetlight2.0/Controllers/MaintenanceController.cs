using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs.MaintenanceDtos;
using MyStreetlight2._0.Models.Enums;
using MyStreetlight2._0.Services.CommonDataService;
using MyStreetlight2._0.Services.LogServices;
using MyStreetlight2._0.Services.MaintenanceServices;
using MyStreetlight2._0.Services.StreetlightService;
using MyStreetlight2._0.ViewModels;

namespace MyStreetlight2._0.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<MaintenanceController> _logger;
        private readonly IStreetlightService _streetlightService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICommonDataService _commonDataService;
        private readonly ILogService _logService;
        //private readonly LightUpdateFlag _flag;

        public MaintenanceController(
            AppDbContext dbContext,
            ILogger<MaintenanceController> logger,
            IStreetlightService streetlightService,
            ICommonDataService commonDataService,
            ILogService logService,
            IMaintenanceService maintenanceService
            //LightUpdateFlag flag
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _streetlightService = streetlightService;
            _commonDataService = commonDataService;
            _logService = logService;
            _maintenanceService = maintenanceService;
            //_flag = flag;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FaultyLights()
        {
            try
            {
                // All Faulted Lights
                var faultyLights = await _maintenanceService.GetAllFaultyLights(null);

                // All In Process Lights
                var acknowledgedInProcessLights = await _maintenanceService.GetAllFaultyLights((int)FaultyLightStatus.Acknowledged) ?? new List<FaultyLightDto>();
                var assigesInProcessLights = await _maintenanceService.GetAllFaultyLights((int)FaultyLightStatus.Assigned) ?? new List<FaultyLightDto>();
                var inProcessLights = acknowledgedInProcessLights.Concat(assigesInProcessLights).ToList();

                // All Repaired Lights
                var repairedLights = await _maintenanceService.GetAllFaultyLights((int)FaultyLightStatus.Repaired);

                //Console.WriteLine(JsonConvert.SerializeObject(faultyLights));

                var faultyLightsViewData = new FaultyLightListViewModel
                {
                    FaultyLights = faultyLights ?? new List<FaultyLightDto>(),
                    InProcessLights = inProcessLights ?? new List<FaultyLightDto>(),
                    RepairedLights = repairedLights ?? new List<FaultyLightDto>()
                };

                //Console.WriteLine(JsonConvert.SerializeObject(faultyLightsViewData));
                return View(faultyLightsViewData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching Faulty light data: {ex.Message}");

                return StatusCode(500, "An error occurred while fetching Faulty light data.");
            }
        }

        public async Task<IActionResult> MarkAsAcknowledged(FaultyLightMaintenanceLogDto logData)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdString, out var userId))
                {
                    TempData["ErrorFeedback"] = "Invalid user identifier";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                // Add log entry
                var logAdded = await _logService.AddFaultyLightMaintenanceLog(logData.FaultId, userId, "Acknowledged", logData.Remark, (int)FaultyLightStatus.Acknowledged);

                // Update status
                var statusUpdated = await _maintenanceService.UpdateFaultyLightStatus(logData.FaultId, (int)FaultyLightStatus.Acknowledged);

                if (logAdded && statusUpdated)
                {
                    await transaction.CommitAsync();
                    TempData["SuccessFeedback"] = "Faulty Light Acknowledged Successfully";
                    return RedirectToAction("FaultyLights", "Maintenance");
                }

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Acknowledge Faulty Light";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Acknowledge Faulty Light");

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Acknowledge Faulty Light";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> MarkAsAssigned(FaultyLightMaintenanceLogDto logData)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdString, out var userId))
                {
                    TempData["ErrorFeedback"] = "Invalid user identifier";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                // Add log entry
                var logAdded = await _logService.AddFaultyLightMaintenanceLog(logData.FaultId, userId, "Assigned", logData.Remark, (int)FaultyLightStatus.Assigned);

                // Update status
                var statusUpdated = await _maintenanceService.UpdateFaultyLightStatus(logData.FaultId, (int)FaultyLightStatus.Assigned);

                if (logAdded && statusUpdated)
                {
                    await transaction.CommitAsync();
                    TempData["SuccessFeedback"] = "Faulty Light Assigned Successfully";
                    return RedirectToAction("FaultyLights", "Maintenance");
                }

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Assigning Faulty Light";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Assigning Faulty Light");

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Assigning Faulty Light";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> MarkAsRepaired(FaultyLightMaintenanceLogDto logData)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdString, out var userId))
                {
                    TempData["ErrorFeedback"] = "Invalid user identifier";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                // Add log entry
                var logAdded = await _logService.AddFaultyLightMaintenanceLog(logData.FaultId, userId, "Repaired", logData.Remark, (int)FaultyLightStatus.Repaired);

                // Update status
                var statusUpdated = await _maintenanceService.UpdateFaultyLightStatus(logData.FaultId, (int)FaultyLightStatus.Repaired);

                if (logAdded && statusUpdated)
                {
                    await transaction.CommitAsync();

                    TempData["SuccessFeedback"] = "Faulty Light Repaired Successfully";
                    return RedirectToAction("FaultyLights", "Maintenance");
                }

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Repairing Faulty Light";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Repairing Faulty Light");

                await transaction.RollbackAsync();
                TempData["ErrorFeedback"] = "Error while Repairing Faulty Light";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> ViewLightMaintenanceLogs(int faultId)
        {
            try
            {
                if (faultId == null)
                {
                    TempData["ErrorFeedback"] = "faulty Id Required.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var logData = await _maintenanceService.GetFaultyLightMaintenanceLogsAsync(faultId);

                if (logData == null)
                {
                    TempData["ErrorFeedback"] = "No logs found for the selected faulty light.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                return PartialView("_LightMaintenanceLogsPartial", logData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching Maintenance Logs: {ex.Message}");
                TempData["ErrorFeedback"] = "An error occurred while fetching Maintenance Logs.";

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> TechnicianDashboard()
        {
            return View();
        }
    }
}
