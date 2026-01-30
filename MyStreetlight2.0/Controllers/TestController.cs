using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streetlight2._0.Data;

namespace Streetlight2._0.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly AppDbContext _dbContext;

        public TestController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var data = await _dbContext.LightsMasters
        //        .Where(x => x.Response != null || x.Response != -1000)
        //        .Select(x => new
        //        {
        //            x.GatewayId,
        //            x.NodeId,
        //            x.Response
        //        })
        //        .ToListAsync();

        //    var newData = await _dbContext.LightLiveData.ToListAsync();
        //    var lightData = await _dbContext.LightsMasters.ToListAsync();

        //    foreach (var item in data)
        //    {
        //        var liveData = newData.FirstOrDefault(x => x.GatewayId == item.GatewayId);
        //        var liveData2 = lightData.FirstOrDefault(x => x.GatewayId == item.GatewayId);
        //        if (liveData != null)
        //        {
        //            liveData.LightStatus = item.Response == 2 ? liveData.LightStatus : item.Response;
        //            liveData2.Response = -1000;
        //        }
        //    }

        //    return View();
        //}
        public class GatewayDto
        {
            public string GatewayId { get; set; }
            public int NodeId { get; set; }
            public int Response { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            var random = new Random();

            // Get all gateways (max 25)
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

            // Shuffle to get random order
            var shuffledData = allData.OrderBy(x => random.Next()).ToList();

            // Process one by one
            foreach (var item in shuffledData)
            {
                var liveData = newData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);
                var liveData2 = lightData.FirstOrDefault(x => x.GatewayId == item.GatewayId && x.NodeId == item.NodeId);


                if (liveData != null && liveData2 != null)
                {
                    liveData.UpdatedAt = DateTime.Now;
                    liveData.LightStatus = (item.Response == 2 || item.Response == -1000) ? liveData.LightStatus : item.Response;
                    liveData2.Response = -1000;
                }

                // Save after each gateway
                await _dbContext.SaveChangesAsync();

                // Wait random time (3–9 sec) before next gateway
                int delay = random.Next(2000, 6000);
                await Task.Delay(delay);
            }

            return View();
        }


    }
}
