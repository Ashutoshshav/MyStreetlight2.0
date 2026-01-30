using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetlight2._0.Models.Misc
{
    public partial class MiscValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        [StringLength(50)]
        public string ValueName { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
