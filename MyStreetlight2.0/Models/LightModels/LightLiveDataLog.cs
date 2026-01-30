using System;
using System.Collections.Generic;

namespace MyStreetlight2._0.Models.LightModels;

public partial class LightLiveDataLog
{
    public int RecordId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? LightId { get; set; }

    public string? GatewayId { get; set; }

    public int? NodeId { get; set; }

    public string? MacId { get; set; }

    public decimal? Comin { get; set; }
}
