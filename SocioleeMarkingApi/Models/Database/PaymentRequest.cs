using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class PaymentRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PaymentReference { get; set; } = null!;

    public bool ActivatedSubscription { get; set; }

    public int Amount { get; set; }

    public DateTime Created { get; set; }

    public virtual User User { get; set; } = null!;
}
