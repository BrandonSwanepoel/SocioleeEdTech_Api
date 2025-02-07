using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Models
{
    public class SubscriptionPaymentDetails
	{
		public SubscriptionPaymentDetails(Guid token, string status, DateTime? expiryDate, PaymentPackage paymentPackage)
		{
			Token = token;
			Status = status;
			ExpiryDate = expiryDate;
			PaymentPackage = paymentPackage;
		}
		public Guid Token { get; set; }
		public string Status { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public PaymentPackage PaymentPackage { get; set; }
	}
}

