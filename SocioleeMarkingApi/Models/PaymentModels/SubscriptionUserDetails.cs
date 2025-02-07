namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class SubscriptionUserDetails
	{
		public Guid? Id { get; set; }
		public string BusinessName { get; set; }
		public string Email { get; set; }
		public bool Monthly { get; set; }
		public int Amount{ get; set; }
	}
}

