using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs;
using MyStreetlight2._0.Filters.PasswordCheckFilter;
using MyStreetlight2._0.Services.CommonDataService;
using MyStreetlight2._0.Services.GatewayService;
using MyStreetlight2._0.Services.StreetlightService;
using MyStreetlight2._0.ViewModels;
using MyStreetlight2._0.Utilities;

namespace MyStreetlight2._0.Controllers
{
    public class GatewayController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GatewayController> _logger;
        private readonly ICommonDataService _commonDataService;
        private readonly IStreetlightService _streetlightService;
        private readonly IGatewayService _gatewayService;
        //private readonly LightUpdateFlag _flag;

        public GatewayController
        (
            AppDbContext dbContext,
            ILogger<GatewayController> logger,
            ICommonDataService commonDataService,
            IStreetlightService streetlightService,
            IGatewayService gatewayService
            //LightUpdateFlag flag
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _commonDataService = commonDataService;
            _streetlightService = streetlightService;
            _gatewayService = gatewayService;
            //_flag = flag;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                //await ReloadPage();

                var gateways = await _gatewayService.GetAllInstalledGateways();

                var data = new GatewayListViewModel
                {
                    Gateways = gateways
                };

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching Gateway data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching Gateway data.");
            }
        }

        public async Task<IActionResult> Gateway(string gatewayId)
        {
            try
            {
                //await ReloadPage();

                if (gatewayId.IsNullOrEmpty())
                {
                    return BadRequest("Gateway ID is required");
                }

                var gateway = await _dbContext.LightsMasters
                    .Where(g => g.GatewayId == gatewayId)
                    .ToListAsync();

                if (gateway == null || !gateway.Any())
                {
                    return NotFound("Gateway not found or has no associated lights");
                }

                var macs = new List<MacSummaryViewModel>();

                foreach (var d in gateway)
                {
                    var liveData = await _streetlightService.GetLightLiveDataByGatewayIdAndNodeId(d.GatewayId, d.NodeId ?? 0);
                    var cONMin = await _streetlightService.GetCumulativeONMinByGatewayIdAndNodeId(d.GatewayId, d.NodeId ?? 0);
                    //Console.WriteLine(JsonConvert.SerializeObject(liveData));

                    macs.Add(new MacSummaryViewModel
                    {
                        LightId = d.LightId,
                        NodeId = d.NodeId,
                        MacId = d.MacId,
                        LightSts = liveData?.LightStatus,
                        Ampere = liveData?.Ampere ?? 0,
                        LastUpdate = liveData?.UpdatedAt ?? DateTime.MaxValue,
                        CumOnHour = Helper.ConvertCoONMinTOCoONHour(cONMin)
                    });
                }

                var gatewayData = await _gatewayService.GetGatewayDataByIdAsync(gatewayId);

                var data = new GatewayDashboardViewModel
                {
                    GatewayId = gateway.First().GatewayId,
                    Address = gateway.First().Address,
                    LastUpdate = gatewayData.UpdatedAt ?? DateTime.MaxValue,
                    RadioStatus = gatewayData.RadioStatus != null ? gatewayData.RadioStatus.Replace("%", " ") : null,
                    Macs = macs
                };

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching Gateway data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching Gateway data.");
            }
        }

        private async Task ReloadPage()
        {
            var random = new Random();

            var allData = await _dbContext.LightsMasters
                .Where(x => x.Response != null || x.Response != -1000)
                .Select(x => new
                {
                    x.GatewayId,
                    NodeId = x.NodeId ?? 0,
                    x.Response
                })
                .ToListAsync();

            var newData = await _dbContext.LightLiveData.ToListAsync();
            var lightData = await _dbContext.LightsMasters.ToListAsync();

            var selectedLights = allData.OrderBy(x => random.Next()).Take(15).ToList();

            foreach (var item in selectedLights)
            {
                var liveData = newData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);
                var liveData2 = lightData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);

                if (liveData != null && liveData2 != null)
                {
                    liveData.UpdatedAt = DateTime.Now;
                    liveData.LightStatus = (item.Response == 2 || item.Response == -1000)
                        ? liveData.LightStatus
                        : item.Response;

                    if (item.Response == 2 || item.Response == -1000)
                    {
                        // Keep previous value
                        continue;
                    }
                    else if (item.Response == 1)
                    {
                        // Random between 0.4 and 0.5
                        liveData.Ampere = (decimal)(random.NextDouble() * (0.5 - 0.4) + 0.4);
                    }
                    else if (item.Response == 0)
                    {
                        // Random between 0.1 and 0.15
                        liveData.Ampere = (decimal)(random.NextDouble() * (0.15 - 0.1) + 0.1);
                    }

                    liveData2.Response = -1000;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        [HttpPost]
        [ServiceFilter(typeof(PasswordCheckFilter))]
        public async Task<IActionResult> SaveActionCommandForGateway(string gatewayId, int actionId)
        {
            //Console.WriteLine(gatewayId + " " + actionId);
            try
            {
                //_flag.IsHigh = true;
                //_flag.TaskCount = 10;

                var gatewayLights = await _dbContext.LightsMasters
                    .Where(l => l.GatewayId == gatewayId)
                    .ToListAsync();

                if (gatewayLights == null || !gatewayLights.Any())
                {
                    TempData["ErrorFeedback"] = "Light not found";
                    return RedirectToAction("Gateway", new { gatewayId = gatewayId });
                }

                foreach (var light in gatewayLights)
                {
                    light.Response = actionId; // or whatever property
                }

                await _dbContext.SaveChangesAsync();

                TempData["SuccessFeedback"] = $"Action Command sent successfully for {gatewayId}";
                return RedirectToAction("Gateway", new { gatewayId = gatewayId });
            }
            catch (Exception ex)
            {
                TempData["ErrorFeedback"] = "An error occurred while saving action command for gateway.";
                _logger.LogError(ex, $"Error while saving action command: {ex.Message}");
                return RedirectToAction("Gateway", new { gatewayId = gatewayId });
            }
        }
    }
}
