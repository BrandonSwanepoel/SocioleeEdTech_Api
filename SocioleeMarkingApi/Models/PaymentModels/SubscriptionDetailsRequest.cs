using System;
namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class SubscriptionDetailsRequest
	{
		public string version { get; set; } = string.Empty;
		public string signature { get; set; } = string.Empty;
	}
}

