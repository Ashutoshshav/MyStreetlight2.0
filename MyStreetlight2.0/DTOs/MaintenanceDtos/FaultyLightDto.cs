namespace MyStreetlight2._0.DTOs.MaintenanceDtos
{
    public class FaultyLightDto
    {
        public int FaultId { get; set; }
        public string LightId { get; set; }
        public string GatewayId { get; set; }
        public int NodeId { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        // If you also need logs
        public DateTime FaultAt { get; set; }
        public string FaultType { get; set; }
        public int? Status { get; set; }
    }
}
