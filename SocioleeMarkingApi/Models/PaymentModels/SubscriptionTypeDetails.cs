namespace SocioleeMarkingApi.Models.PaymentModels
{
	public class SubscriptionTypeDetails
	{
		public bool PremiumSubscription { get; set; } = false;
		public bool ActiveSubscription { get; set; } = false;

		public SubscriptionTypeDetails(bool premiumSubscription, bool activeSubscription = false) {
			PremiumSubscription = premiumSubscription;
			ActiveSubscription = activeSubscription;
		}
	}
}