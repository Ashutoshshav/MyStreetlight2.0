using System.ComponentModel.DataAnnotations;

namespace Streetlight2._0.Models.MaintenanceModels
{
    public partial class FaultyLightMaintenanceLog
    {
        [Key]
        public int LogId { get; set; }

        public int FaultId { get; set; }

        public int UserId { get; set; }

        public string Action { get; set; }

        public string? Remark { get; set; }

        public int ChangedStatus { get; set; }

        public DateTime LoggedAt { get; set; }
    }
}
