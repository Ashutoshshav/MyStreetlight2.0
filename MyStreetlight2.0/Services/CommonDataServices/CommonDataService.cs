using Microsoft.EntityFrameworkCore;
using Streetlight2._0.Data;

namespace Streetlight2._0.Services.CommonDataService
{
    public class CommonDataService : ICommonDataService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CommonDataService> _logger;

        public CommonDataService(
            AppDbContext dbContext,
            ILogger<CommonDataService> logger
        )
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> GetLightStsNameByStsId(int stsId)
        {
            try
            {
                var sts = await _dbContext.LightStsMasters.FirstOrDefaultAsync(s => s.StatusId == stsId);

                if (sts == null)
                {
                    return "Status not found";
                }

                return sts.StatusName;
            }
            catch (Exception)
            {
                return "Error retrieving status";
            }
        }
    }
}
