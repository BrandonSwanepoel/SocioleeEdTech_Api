using System;
namespace SocioleeMarkingApi.Models.AuthModels
{
	public class UserVerificationCode
	{
		public Guid UserId { get; set; }
		public string Code { get; set; } = string.Empty;
		public bool ResetPassword { get; set; }
	}

	public class UserCouponRedeemed
	{
		public Guid UserId { get; set; }
		public string Code { get; set; } = string.Empty;
	}
}

