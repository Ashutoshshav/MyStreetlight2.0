namespace MyStreetlight2._0.DTOs.LightDtos
{
    public class StreetlightDto
    {
        public string LightId { get; set; } = null!;

        public string GatewayId { get; set; } = null!;

        public string MacId { get; set; } = null!;

        public string PollId { get; set; } = null!;

        public string? Zone { get; set; }

        public string? Ward { get; set; }

        public string? Address { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public int? LightWattage { get; set; }
    }
}
