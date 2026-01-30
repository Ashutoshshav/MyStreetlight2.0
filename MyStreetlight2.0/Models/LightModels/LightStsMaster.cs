using System;
using System.Collections.Generic;

namespace MyStreetlight2._0.Models.LightModels;

public partial class LightStsMaster
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusDescription { get; set; }
}
