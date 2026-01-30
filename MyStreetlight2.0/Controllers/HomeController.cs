using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Streetlight2._0.Data;
using Streetlight2._0.DTOs;
using Streetlight2._0.DTOs.LightDtos;
using Streetlight2._0.DTOs.MicsDtos;
using Streetlight2._0.Models;
using Streetlight2._0.Models.GatewayModels;
using Streetlight2._0.Models.Misc;
using Streetlight2._0.Services.StreetlightService;
using Streetlight2._0.Utilities;
using Streetlight2._0.ViewModels;
using WebPush;

namespace Streetlight2._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IStreetlightService _streetlightService;
        private static List<PushSubscription> _subscriptions = new();
        private readonly CookieDecoder _cookieDecoder;

        public HomeController(AppDbContext dbContext,
            ILogger<HomeController> logger,
            IStreetlightService streetlightService,
            CookieDecoder cookieDecoder)
        {
            _dbContext = dbContext;
            _logger = logger;
            _streetlightService = streetlightService;
            _cookieDecoder = cookieDecoder;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lighWithtLiveData = await _streetlightService.GetAllLightsWithLiveDataAsync();

                var lights = lighWithtLiveData
                    .Select(d => new LightDataDto
                    {
                        LightId = d.LightId,
                        MacId = d.MacId,
                        GatewayId = d.GatewayId,
                        NodeId = d.NodeId,
                        LightStatus = d.LightStatus,
                        Latitude = d.Latitude,
                        Longitude = d.Longitude,
                        Address = d.Address,
                    })
                    .ToList();

                var wardWiseLightData = lighWithtLiveData
                    .GroupBy(d => d.Ward)
                    .Select(d => new WardWiseLightDto
                    {
                        Ward = d.Key,
                        TotalLightCount = d.Count(c => c.Ward == d.Key && c.LightStatus != null), // for come into count there should be any value in Light status
                        ActiveLightCount = d.Count(c => c.Ward == d.Key && (c.LightStatus == 1 || c.LightStatus == 0)), // On and Off lights considering as Active light
                        FaultyLightCount = d.Count(c => c.Ward == d.Key && c.LightStatus == 2),
                        NoPowerLightCount = d.Count(c => c.Ward == d.Key && c.LightStatus == 3)
                    })
                    .OrderBy(d => d.Ward)
                    .ToList();

                var data = new HomeIndexViewModel
                {
                    TotalLightCount = lighWithtLiveData.Count(d => d.LightStatus != null), // for come into count there should be any value in Light status
                    ActiveLightCount = lighWithtLiveData.Count(d => (d.LightStatus == 1 || d.LightStatus == 0)), // On and Off lights considering as Active light
                    FaultyLightCount = lighWithtLiveData.Count(d => d.LightStatus == 2),
                    NoPowerLightCount = lighWithtLiveData.Count(d => d.LightStatus == 3),
                    WardWiseLight = wardWiseLightData,
                    Lights = lights
                };

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while rendering page");

                throw;
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> data()
        {
            try
            {
                var lighWithtLiveData = await _streetlightService.GetAllLightsWithLiveDataAsync();

                var wardWiseLightData = lighWithtLiveData
                    .GroupBy(d => d.Ward)
                    .Select(d => new WardWiseLightDto
                    {
                        Ward = d.Key,
                        TotalLightCount = d.Count(c => c.Ward == d.Key),
                        ActiveLightCount = d.Count(c => c.Ward == d.Key && (c.LightStatus == 1 || c.LightStatus == 0)),
                        FaultyLightCount = d.Count(c => c.Ward == d.Key && c.LightStatus == 2),
                        NoPowerLightCount = d.Count(c => c.Ward == d.Key && c.LightStatus == 3)
                    })
                    .ToList();

                var data = new HomeIndexViewModel
                {
                    TotalLightCount = lighWithtLiveData.Count(),
                    ActiveLightCount = lighWithtLiveData.Count(d => (d.LightStatus == 1 || d.LightStatus == 0)),
                    FaultyLightCount = lighWithtLiveData.Count(d => d.LightStatus == 2),
                    NoPowerLightCount = lighWithtLiveData.Count(d => d.LightStatus == 3),
                    WardWiseLight = wardWiseLightData
                };

                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while rendering page");

                throw;
            }
        }

        [HttpPost]
        public IActionResult SaveSubscription([FromBody] PushSubscription subscription)
        {
            _subscriptions.Add(subscription);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string title, string message)
        {
            var vapidDetails = new VapidDetails(
                "mailto:your@email.com",
                "BHL6kKJxv1xTOUZmL0Y1hDdPj1O87mTXj6qhdQ6aO9JjKxFHVh4kqN2E6sGykq1LvwSkq3UgRUaBgHd5k4XhYdg",
                "BHL6kKJxv1xTlkJrNkjKJkj99ws87mTXj6qhdQ6aO9JjKxFHVh4kqN2E6sGykq1LvwSkq3UgRUaBgHd5k4XhYdg"
            );

            var webPushClient = new WebPushClient();

            foreach (var sub in _subscriptions)
            {
                var payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    title,
                    body = message
                });

                await webPushClient.SendNotificationAsync(sub, payload, vapidDetails);
            }

            return Ok("Sent!");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DecodeCookie()
        {
            var cookie = Request.Cookies["StreetlightToken"];
            if (cookie == null) return BadRequest("No auth cookie");

            var principal = _cookieDecoder.DecodeCookie(cookie);

            //if (principal != null)
            //{
            //    Console.WriteLine($"Identity Name: {principal.Identity?.Name}");
            //    Console.WriteLine("Claims: ");

            //    foreach (var claim in principal.Claims)
            //    {
            //        Console.WriteLine($" - {claim.Type}: {claim.Value}");
            //    }
            //}

            if (principal != null)
            {
                var permissions = User.Claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                return Ok(new
                {
                    Name = principal.Identity?.Name,
                    Permissions = permissions
                });
            }

            return BadRequest("Failed to decode cookie");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
