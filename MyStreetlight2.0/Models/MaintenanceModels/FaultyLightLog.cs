namespace Streetlight2._0.Models.MaintenanceModels
{
    public partial class FaultyLightLog
    {
        public int RecordId { get; set; }

        public string? LightId { get; set; }

        public string? GatewayId { get; set; } = null!;

        public int? NodeId { get; set; } = null!;

        public string? MacId { get; set; }

        public string? LightStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? Status { get; set; }
    }
}
