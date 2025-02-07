using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public byte[]? PasswordHash { get; set; }

    public string? PasswordResetToken { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenCreated { get; set; }

    public DateTime? RefreshTokenExpires { get; set; }

    public DateTime? ResetTokenExpires { get; set; }

    public string? VerificationToken { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public Guid RoleId { get; set; }

    public int CouponRedeemed { get; set; }

    public virtual ICollection<ContentAsset> ContentAssets { get; } = new List<ContentAsset>();

    public virtual ICollection<ContentForm> ContentForms { get; } = new List<ContentForm>();

    public virtual ICollection<Coupon> Coupons { get; } = new List<Coupon>();

    public virtual ICollection<DesignChangeRequest> DesignChangeRequests { get; } = new List<DesignChangeRequest>();

    public virtual ICollection<InstitutionLecturer> InstitutionLecturers { get; } = new List<InstitutionLecturer>();

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<PaymentDetail> PaymentDetails { get; } = new List<PaymentDetail>();

    public virtual ICollection<PaymentRequest> PaymentRequests { get; } = new List<PaymentRequest>();

    public virtual RoleType Role { get; set; } = null!;

    public virtual ICollection<StudentProjectAssignment> StudentProjectAssignments { get; } = new List<StudentProjectAssignment>();

    public virtual ICollection<StudentProject> StudentProjects { get; } = new List<StudentProject>();

    public virtual ICollection<Student> Students { get; } = new List<Student>();
}
