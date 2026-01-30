namespace Streetlight2._0.ViewModels
{
    public class LightPageViewModel
    {
        // Identification
        public string GatewayId { get; set; }
        public string LightId { get; set; }
        public int? NodeId { get; set; }
        public string MacId { get; set; }
        public string Zone { get; set; }
        public string Ward { get; set; }
        public string PollNo { get; set; }
        public int Wattage { get; set; }
        public decimal Current { get; set; }
        public string LightOnHour { get; set; }

        // Location
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        // Status
        public string PowerStatus { get; set; }
        public string ConnectionStatus { get; set; }
        public DateTime LastUpdate { get; set; }

        // Energy Consumption
        public List<EnergyDataPoint> EnergyConsumption { get; set; }
    }

    public class EnergyDataPoint
    {
        public string Time { get; set; }
        public double Value { get; set; }
    }
}
