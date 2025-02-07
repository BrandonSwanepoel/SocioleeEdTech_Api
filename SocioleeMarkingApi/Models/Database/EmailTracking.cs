using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class EmailTracking
{
    public Guid Id { get; set; }

    public bool Subscribed { get; set; }

    public string Type { get; set; } = null!;

    public Guid UserId { get; set; }
}
