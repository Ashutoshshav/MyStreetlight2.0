using System;
using System.Collections.Generic;

namespace Streetlight2._0.Models.UserModels;

public partial class UserPermission
{
    public int RecordId { get; set; }

    public int UserId { get; set; }

    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
