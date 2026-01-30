using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Streetlight2._0.Data;
using Streetlight2._0.DTOs;
using Streetlight2._0.Filters.PasswordCheckFilter;
using Streetlight2._0.Services.CommonDataService;
using Streetlight2._0.Services.StreetlightService;
using Streetlight2._0.ViewModels;
using Streetlight2._0.Utilities;

namespace Streetlight2._0.Controllers
{
    public class StreetlightController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StreetlightController> _logger;
        private readonly IStreetlightService _streetlightService;
        private readonly ICommonDataService _commonDataService;
        //private readonly LightUpdateFlag _flag;

        public StreetlightController(
            AppDbContext dbContext,
            ILogger<StreetlightController> logger,
            IStreetlightService streetlightService,
            ICommonDataService commonDataService
            //LightUpdateFlag flag
        ) 
        {
            _dbContext = dbContext;
            _logger = logger;
            _streetlightService = streetlightService;
            _commonDataService = commonDataService;
            //_flag = flag;
        }

        public async Task<IActionResult> Light(string lightId)
        {
            try
            {
                var light = await _dbContext.LightsMasters
                    .Select(g => new LightPageViewModel
                    {
                        GatewayId = g.GatewayId,
                        LightId = g.LightId,
                        NodeId = g.NodeId,
                        MacId = g.MacId,
                        Zone = g.Zone,
                        Ward = g.Ward,
                        PollNo = g.PollId,
                        Wattage = g.LightWattage ?? 0,
                        Address = g.Address ?? "N/A",
                        Latitude = g.Latitude,
                        Longitude = g.Longitude
                    })
                    .FirstOrDefaultAsync(l => l.LightId == lightId);

                if (light == null)
                {
                    return NotFound("Light not found");
                }

                var lightLiveData = await _streetlightService.GetLightLiveDataByGatewayIdAndNodeId(light.GatewayId, light.NodeId ?? 0);

                if (lightLiveData == null)
                {
                    light.Current = 0;
                    light.LastUpdate = DateTime.MaxValue;
                    light.PowerStatus = "";
                }
                else
                {
                    light.Current = lightLiveData.Ampere ?? 0;
                    light.LastUpdate = lightLiveData.UpdatedAt ?? DateTime.MaxValue;
                    light.PowerStatus = lightLiveData.LightStatus;
                }

                var cONMin = await _streetlightService.GetCumulativeONMinByGatewayIdAndNodeId(light.GatewayId, light.NodeId ?? 0);

                light.LightOnHour = Helper.ConvertCoONMinTOCoONHour(cONMin);

                return View(light);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching light data: {ex.Message}");

                return StatusCode(500, "An error occurred while fetching light data.");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(PasswordCheckFilter))]
        public async Task<IActionResult> SaveActionCommand(string gatewayId, int nodeId, string lightId, int actionId)
        {
            //Console.WriteLine(gatewayId + " " + lightId + " " + actionId);
            try
            {
                //_flag.IsHigh = true;
                //_flag.TaskCount = 15;

                var lightData = await _dbContext.LightsMasters.FirstOrDefaultAsync(l => l.GatewayId == gatewayId && l.NodeId == nodeId);

                if (lightData == null)
                {
                    TempData["ErrorFeedback"] = "Light not found";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                lightData.Response = actionId;
                await _dbContext.SaveChangesAsync();

                TempData["SuccessFeedback"] = $"Action Command sent successfully for {lightId}";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorFeedback"] = "An error occurred while saving action command.";
                _logger.LogError(ex, $"Error while saving action command: {ex.Message}");

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(PasswordCheckFilter))]
        public async Task<IActionResult> SaveActionCommandForAll(int actionId)
        {
            try
            {
                //_flag.IsHigh = true;
                //_flag.TaskCount = 12;

                if (actionId < 0 || actionId == null)
                {
                    TempData["ErrorFeedback"] = "Action command required.";

                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var lights = await _dbContext.LightsMasters.ToListAsync();

                if (!lights.Any() || actionId == null)
                {
                    TempData["ErrorFeedback"] = "Any Lights does not exist.";

                    return Redirect(Request.Headers["Referer"].ToString());
                }

                foreach (var light in lights)
                {
                    light.Response = actionId;
                }

                await _dbContext.SaveChangesAsync();

                TempData["SuccessFeedback"] = "Action Command sent successfully for All Lights";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorFeedback"] = "An error occurred while saving action command for All Lights.";
                _logger.LogError(ex, $"Error while saving action command for All Lights: {ex.Message}");

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
