using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class PaymentPackage
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Amount { get; set; }

    public bool Monthly { get; set; }

    public int DesignAmount { get; set; }

    public virtual ICollection<PaymentDetail> PaymentDetails { get; } = new List<PaymentDetail>();
}
