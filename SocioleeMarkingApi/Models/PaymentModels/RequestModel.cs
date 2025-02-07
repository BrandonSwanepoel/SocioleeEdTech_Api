using Microsoft.AspNetCore;

namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class RequestModel
	{
		//Merchant details
		public string merchant_id { get; set; }
		public string merchant_key { get; set; }
		public string? return_url { get; set; } = string.Empty;
		public string? cancel_url { get; set; } = string.Empty;
		public string? notify_url { get; set; } = string.Empty;

		//Customer details
		public string? name_first { get; set; } = string.Empty;
		public string? name_last { get; set; } = string.Empty;
		public string? email_address { get; set; } = string.Empty;
		public string? cell_number { get; set; } = string.Empty;

		//Transaction details
		public string? m_payment_id { get; set; } = string.Empty;
		public double amount { get; set; }
		public string item_name { get; set; } = string.Empty;
		public string? item_description { get; set; } = string.Empty;

		//Transaction options
		public int email_confirmation { get; set; }
		public string? confirmation_address { get; set; } = string.Empty;

		//Payment methods
		public string? payment_method { get; set; } = string.Empty;

		//Recurring details
		public int subscription_type { get; set; }
		public DateOnly? billing_date { get; set; }
		public double? recurring_amount { get; set; }
		public int frequency { get; set; }
		public int cycles { get; set; }
		//public bool subscription_notify_email { get; set; }
		//public bool subscription_notify_webhook { get; set; }
		//public bool subscription_notify_buyer { get; set; }
	}
}

