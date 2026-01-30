using System;
using System.Collections.Generic;

namespace MyStreetlight2._0.Models.LightModels;

public partial class LightsMaster
{
    public int RecordId { get; set; }

    public string LightId { get; set; } = null!;

    public string GatewayId { get; set; } = null!;

    public int? NodeId { get; set; } = null!;

    public string MacId { get; set; } = null!;

    public int Response { get; set; }

    public string? PollId { get; set; }

    public string? Zone { get; set; }

    public string? Ward { get; set; }

    public string? Address { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public int? LightWattage { get; set; }
}
