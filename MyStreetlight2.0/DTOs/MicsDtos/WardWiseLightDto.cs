namespace Streetlight2._0.DTOs.MicsDtos
{
    public class WardWiseLightDto
    {
        public string Ward { get; set; }

        public int TotalLightCount { get; set; }

        public int ActiveLightCount { get; set; }

        public int FaultyLightCount { get; set; }

        public int NoPowerLightCount { get; set; }
    }
}
