namespace Streetlight2._0.DTOs.GatewayDtos
{
    public class GatewayDto
    {
        public string Id { get; set; } = null!;

        public string? Address { get; set; }

        public string? Zone { get; set; }

        public string? Ward { get; set; }

        public int? LightCount { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public int? OnLightCount { get; set; }

        public int? OffLightCount { get; set; }

        public int? NGLightCount { get; set; }

        public int? NCLightCount { get; set; }

        public string? RadioStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
