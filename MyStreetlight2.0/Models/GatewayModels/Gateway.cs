using System.ComponentModel.DataAnnotations;

namespace MyStreetlight2._0.Models.GatewayModels
{
    public class Gateway
    {
        [Key]
        public int RecordId { get; set; }

        [MaxLength(50)]
        public string? GatewayId { get; set; }

        [MaxLength(50)]
        public string? MacId { get; set; }

        [MaxLength(50)]
        public string? Zone { get; set; }

        [MaxLength(50)]
        public string? Ward { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Latitude { get; set; }

        [MaxLength(50)]
        public string? Longitude { get; set; }

        public int? ConnectedLights { get; set; }

        public int? Response { get; set; }

        [MaxLength(200)]
        public string? RadioStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
