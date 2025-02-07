namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class ResponseModel
	{
		public string? m_payment_id { get; set; }
		public string pf_payment_id { get; set; }
		public string payment_status { get; set; }
		public string item_name { get; set; }
		public string? item_description { get; set; }
		public double? amount_gross { get; set; }
		public double? amount_fee { get; set; }
		public double? amount_net { get; set; }
		//Customer details
		public string? name_first { get; set; }
		public string? name_last { get; set; }
		public string email_address { get; set; }
		//Merchant details
		public string merchant_id { get; set; }

		//Recurring Billing details
		public string token { get; set; }
		public string billing_date { get; set; }
		//Security information
		public string signature { get; set; }
	}
}
