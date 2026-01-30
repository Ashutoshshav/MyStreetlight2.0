using System.ComponentModel.DataAnnotations;

namespace MyStreetlight2._0.Models.MaintenanceModels
{
    public class FaultyLightWithMaintenanceLog
    {
        [Key]
        public int LogId { get; set; }

        public int FaultId { get; set; }

        public string? GatewayId { get; set; } = null!;

        public int? NodeId { get; set; } = null!;

        public string? MacId { get; set; }

        public string? LightStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? Status { get; set; }

        public int UserId { get; set; }

        public string Action { get; set; }

        public string? Remark { get; set; }

        public int ChangedStatus { get; set; }

        public DateTime LoggedAt { get; set; }

        public string? LightId { get; set; }
    }
}
