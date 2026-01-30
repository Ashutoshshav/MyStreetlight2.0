namespace Streetlight2._0.DTOs.LightDtos
{
    public class LightLiveDataDto
    {
        public int RecordId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? LightId { get; set; }

        public string MacId { get; set; }

        public string LightStatus { get; set; }

        public decimal? Ampere { get; set; }
    }
}
