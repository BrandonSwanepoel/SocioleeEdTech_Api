using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class PaymentDetail
{
    public Guid Id { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string PaymentId { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string PfPaymentId { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public Guid Token { get; set; }

    public Guid UserId { get; set; }

    public bool FrequencyMonthly { get; set; }

    public Guid PaymentPackageId { get; set; }

    public virtual PaymentPackage PaymentPackage { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
