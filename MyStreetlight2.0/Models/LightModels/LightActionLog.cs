using System;
using System.Collections.Generic;

namespace Streetlight2._0.Models.LightModels;

public partial class LightActionLog
{
    public int RecordId { get; set; }

    public string? LightId { get; set; }

    public string? CommandFor { get; set; }

    public int UserId { get; set; }

    public string? Action { get; set; }

    public string? ActionRemark { get; set; }

    public DateTime? CreatedAt { get; set; }
}
