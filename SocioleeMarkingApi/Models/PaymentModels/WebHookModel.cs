using System;
namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class WebHookModel
	{
		public string type { get; set; }
		public string token { get; set; }
		public int initial_amount { get; set; }
		public int amount { get; set; }
		public DateOnly next_run { get; set; }
		public int frequency { get; set; }
		public string item_name { get; set; }
		public string item_description { get; set; }
		public string name_first { get; set; }
		public string name_last { get; set; }
		public string email_address { get; set; }
	}
}

