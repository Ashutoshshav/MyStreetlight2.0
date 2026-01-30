using MyStreetlight2._0.DTOs.GatewayDtos;

namespace MyStreetlight2._0.ViewModels
{
    public class GatewayListViewModel
    {
        public List<GatewayDto> Gateways { get; set; } = new List<GatewayDto>();

        public int TotalCount => Gateways.Count;
    }
}
