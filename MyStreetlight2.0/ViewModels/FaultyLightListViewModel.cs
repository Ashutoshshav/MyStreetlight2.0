using Streetlight2._0.DTOs.MaintenanceDtos;

namespace Streetlight2._0.ViewModels
{
    public class FaultyLightListViewModel
    {
        public List<FaultyLightDto> FaultyLights { get; set; }

        public List<FaultyLightDto> InProcessLights { get; set; }

        public List<FaultyLightDto> RepairedLights { get; set; }

        public FaultyLightMaintenanceLogDataDto FaultyLightMaintenanceLogData { get; set; }
    }
}
