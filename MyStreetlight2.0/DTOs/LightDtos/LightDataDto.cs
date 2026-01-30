namespace MyStreetlight2._0.DTOs.LightDtos
{
    public class LightDataDto
    {
        public string LightId { get; set; }

        public string? MacId { get; set; }

        public string GatewayId { get; set; }

        public int? NodeId { get; set; }

        public int? LightStatus { get; set; }

        public decimal? Ampere { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? PollId { get; set; }

        public string? Zone { get; set; }

        public string? Ward { get; set; }

        public string? Address { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public int? LightWattage { get; set; }

        public int Response { get; set; }
    }
}
