using Microsoft.AspNetCore;

namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class Cancel
	{
		public string token { get; set; }
		public string merchant_id { get; set; }
		public string version { get; set; }
		public DateTime timestamp { get; set; }
		public string signature { get; set; }
	}
}

