namespace Streetlight2._0.ViewModels
{
    public class GatewayDashboardViewModel
    {
        public string GatewayId { get; set; }
        //public string GatewayName { get; set; }
        public string Address { get; set; }

        public string RadioStatus { get; set; }

        public DateTime LastUpdate { get; set; }

        public List<MacSummaryViewModel> Macs { get; set; }
    }

    public class MacSummaryViewModel
    {
        public string MacId { get; set; }
        public string LightId { get; set; }
        public int? NodeId { get; set; }
        public bool IsConnected { get; set; }
        public string LightSts { get; set; }
        public int Wattage { get; set; }
        public decimal Current { get; set; }
        public decimal Ampere { get; set; }
        public string CumOnHour { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
