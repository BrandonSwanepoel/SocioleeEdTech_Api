using System;
namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class CancelUserdetails
	{
		public Guid UserId { get; set; }
		public Guid Token { get; set; }
	}
}

