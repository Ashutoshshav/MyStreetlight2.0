using MyStreetlight2._0.DTOs.LightDtos;
using MyStreetlight2._0.DTOs.MicsDtos;

namespace MyStreetlight2._0.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalLightCount { get; set; }

        public int ActiveLightCount { get; set; }

        public int FaultyLightCount { get; set; }

        public int NoPowerLightCount { get; set; }

        public List<WardWiseLightDto> WardWiseLight { get; set; }

        public List<LightDataDto> Lights { get; set; }
    }
}
