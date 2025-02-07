using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class Coupon
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public DateTime Created { get; set; }

    public bool Redeemed { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
