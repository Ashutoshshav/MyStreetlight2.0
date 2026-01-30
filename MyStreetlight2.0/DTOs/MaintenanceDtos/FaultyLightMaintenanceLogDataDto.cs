namespace MyStreetlight2._0.DTOs.MaintenanceDtos
{
    public class FaultyLightMaintenanceLogDataDto
    {
        public int FaultId { get; set; }

        public string? GatewayId { get; set; } = null!;

        public int? NodeId { get; set; } = null!;

        public string? FaultyStatus { get; set; }

        public DateTime FaultedAt { get; set; }

        public int? MaintenanceStatus { get; set; }

        public List<MaintenanceLog> MaintenanceLogs { get; set; }
    }

    public class MaintenanceLog
    {
        public string UserName { get; set; }

        public string Action { get; set; }

        public string? Remark { get; set; }

        public DateTime LoggedAt { get; set; }
    }
}
