using System;
using PayFast;
using PayFast.ApiTypes;
using SocioleeMarkingApi.Enums.Payments;

namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class SubscriptionDetails
	{
		public GenericData<SubscriptionDetail> data { get; set; }
	}
	public class SubscriptionDetail
	{
		public string? businessName { get; set; } = string.Empty;
		public string token { get; set; } = string.Empty;
		public decimal amount { get; set; }
		public int cycles { get; set; }
		public int cycles_complete { get; set; }
		public PaymentStatus status { get; set; }
		public DateTime run_date { get; set; }
		public bool monthly { get; set; }
	}
}

