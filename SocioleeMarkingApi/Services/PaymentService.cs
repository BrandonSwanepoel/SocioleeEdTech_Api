using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PayFast;
using RazorHtmlEmails.Common;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Enums.Payments;
using SocioleeMarkingApi.Models.PaymentModels;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IPaymentService
	{
		Task<string> CreateSubscriptionRequestUrl(SubscriptionUserDetails user);
		Task<bool> CheckIfUserHasAccount(string email);
		Task<bool> AddToDatabase(PayFastNotify payFastNotify);
		bool ValidatePayFastSignature(PayFastNotify payFastNotify);
		Task<bool> ValidPayfastDomain(IPAddress? iPAddress);
		Task<bool> ValidateAmountPaid(string amountGross);
		Task<bool> ValidateData(PayFastNotify payFastNotify);
		Task<bool> Cancel(CancelUserdetails userdetails);
		Task<SubscriptionDetails?> Details(Guid userId);
		Task<bool> SubscriptionExpiryEmail(User user, string expiryDate);
		Task<bool> ActiveSubscription(Guid userId);
		Task<bool> CancelledSubscription(Guid userId);
		Task<SubscriptionTypeDetails> GetSubscriptionTypeDetails(Guid userId);
		Task<bool> HasPremiumSubscription(Guid userId);
		Task<SubscriptionPaymentDetails?> GetSubscriptionPackage(Guid userId);
	}
	public class PaymentService : IPaymentService
	{
		private readonly SocioleeDesignContext _db;
		private readonly IConfiguration _configuration;
		private readonly IUniqueIds _uniqueIds;
		private readonly IRegisterAccountService _registerAccountService;

		public PaymentService(SocioleeDesignContext dbContext, IConfiguration configuration, IUniqueIds uniqueIds, IRegisterAccountService registerAccountService)
		{
			_db = dbContext;
			_configuration = configuration;
			_uniqueIds = uniqueIds;
			_registerAccountService = registerAccountService;
		}

		public async Task<string> CreateSubscriptionRequestUrl(SubscriptionUserDetails user)
		{
			//var userHasAccount = await CheckIfUserHasAccount(user.Email);

			var paymentRequest = SetRequest(user, user.Monthly);
			var url = ToKeyValueURL(paymentRequest);

			var encodedUrl = ToKeyValueEncodedURL(url);
			url += $"&signature={Signature(encodedUrl)}";
			return url;
		}

		private RequestModel SetRequest(SubscriptionUserDetails user, bool monthlySubscription)
		{
			var paymentRequest = new RequestModel
			{
				// Merchant Details
				merchant_id = _configuration.GetValue<string>("MerchantId"),
				merchant_key = _configuration.GetValue<string>("MerchantKey"),
				//merchant_id = _configuration.GetSection("PayFastSettings:MerchantId").Value,
				//merchant_key = _configuration.GetSection("PayFastSettings:MerchantKey").Value,
				return_url =  _configuration.GetSection("PayFastSettings:ReturnUrl").Value,
				cancel_url = _configuration.GetSection("PayFastSettings:CancelUrl").Value,
				notify_url = _configuration.GetSection("PayFastSettings:NotifyUrl").Value,

				// Buyer Details
				email_address = user.Email,

				// Transaction Details
				m_payment_id = Guid.NewGuid().ToString(),
				item_name = $"{user.BusinessName}'s Sociolee Design subscription",
				item_description = "Professional subscription package",

				// Transaction Options
				amount = double.Parse(user.Amount.ToString(), CultureInfo.InvariantCulture),
				email_confirmation = 1,
				confirmation_address = user.Email,

				// Recurring Billing Details
				subscription_type = 1,
				//billing_date = DateOnly.Parse(DateTime.Today.AddMonths(+1).ToString("yyyy'-'MM'-'dd")),
				recurring_amount = double.Parse(user.Amount.ToString(), CultureInfo.InvariantCulture),
				frequency = monthlySubscription ? (int)Frequency.Monthly + 1 : (int)Frequency.Annual + 1,
				cycles = 0,
			};
			return paymentRequest;
		}

		public async Task<bool> CheckIfUserHasAccount(string email)
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && x.VerifiedAt != null);
			if (user == null)
				return false;

			var userPaymentDetails = await _db.PaymentDetails.FirstOrDefaultAsync(x => x.UserId == user.Id);
			return userPaymentDetails != null;
		}

		public async Task<bool> Cancel(CancelUserdetails userdetails)
		{
			using (var httpClient = GetClient())
			{
				//var finalUrl = $"{_configuration.GetSection("PayFastSettings:SubscriptionUrl").Value}/{userdetails.Token}/cancel?testing=true";

				var finalUrl = $"{_configuration.GetSection("PayFastSettings:SubscriptionUrl").Value}/{userdetails.Token}/cancel";

				GenerateSignature(httpClient);

				using (var response = await httpClient.PutAsync(finalUrl, null))
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						await PaymentCancelUpdateDatabase(userdetails.Token);
						return true;
					}

					throw new Exception("Could not fetch subscription");
				}
			}
		}

		public async Task<SubscriptionDetails?> Details(Guid userId)
		{
			var paymentDetails = await _db.PaymentDetails.Where(x => x.UserId == userId).OrderByDescending(p => p.ExpiryDate).FirstOrDefaultAsync();
			if(paymentDetails == null)
			{
				return null;
			}
			var token = paymentDetails.Token.ToString();

			return await Fetch(token);
		}

		public async Task<bool> HasPremiumSubscription(Guid userId)
		{
			return await _db.PaymentDetails.AnyAsync(x => x.UserId == userId);
		}

		public async Task<bool> ActiveSubscription(Guid userId)
		{
			var paymentDetails = await _db.PaymentDetails.OrderByDescending(p => p.ExpiryDate).FirstOrDefaultAsync(x => x.UserId == userId);
			if (paymentDetails == null)
			{
				return false;
			}

			return paymentDetails.ExpiryDate >= CommonFunctions.CurrentDateTime();
		}

		public async Task<SubscriptionTypeDetails> GetSubscriptionTypeDetails(Guid userId)
		{
			bool activeSubscription = false;
			var hasPremiumSubscription = await HasPremiumSubscription(userId);
			if (hasPremiumSubscription == true)
			{
				activeSubscription = await ActiveSubscription(userId);
			}

			return new SubscriptionTypeDetails(hasPremiumSubscription, activeSubscription);
		}

		public async Task<bool> CancelledSubscription(Guid userId)
		{
			var paymentDetails = await _db.PaymentDetails.OrderByDescending(p => p.ExpiryDate).FirstOrDefaultAsync(x => x.UserId == userId);
			if (paymentDetails == null)
			{
				return false;
			}

			return paymentDetails.ExpiryDate < CommonFunctions.CurrentDateTime();
		}

		private async Task<SubscriptionDetails> Fetch(string token)
		{
			using var httpClient = GetClient();
			//var finalUrl = $"{_configuration.GetSection("PayFastSettings:SubscriptionUrl").Value}/{token}/fetch?testing=true";

			var finalUrl = $"{_configuration.GetSection("PayFastSettings:SubscriptionUrl").Value}/{token}/fetch";

			GenerateSignature(httpClient);

			using var response = await httpClient.GetAsync(finalUrl);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				 var val = await response.Deserialize<SubscriptionDetails>();
				return val;
			}

			throw new Exception("Could not fetch subscription");
		}

		protected HttpClient GetClient()
		{
			var httpClient = new HttpClient { BaseAddress = new Uri(_configuration.GetSection("PayFastSettings:SubscriptionUrl").Value) };

			httpClient.DefaultRequestHeaders.Clear();
			//httpClient.DefaultRequestHeaders.Add("merchant-id", _configuration.GetSection("PayFastSettings:MerchantId").Value);
			httpClient.DefaultRequestHeaders.Add("merchant-id", _configuration.GetValue<string>("MerchantId"));
			httpClient.DefaultRequestHeaders.Add("version", "v1");
			httpClient.DefaultRequestHeaders.Add("timestamp", DateTime.UtcNow.ToString("s"));

			return httpClient;
		}

		protected string GenerateSignature(HttpClient httpClient, params KeyValuePair<string, string>[] parameters)
		{
			var dictionary = new SortedDictionary<string, string>();

			foreach (var header in httpClient.DefaultRequestHeaders)
			{
				dictionary.Add(key: header.Key, value: header.Value.First());
			}

			foreach (var keyValuePair in parameters)
			{
				dictionary.Add(key: keyValuePair.Key, value: keyValuePair.Value);
			}

			//if (!string.IsNullOrWhiteSpace(_configuration.GetSection("PayFastSettings:Payfast_PassPhrase").Value))
			if (!string.IsNullOrWhiteSpace(_configuration.GetValue<string>("Payfast_PassPhrase")))
			{
				//dictionary.Add(key: "passphrase", value: _configuration.GetSection("PayFastSettings:Payfast_PassPhrase").Value);
				dictionary.Add(key: "passphrase", value: _configuration.GetValue<string>("Payfast_PassPhrase"));
			}

			var stringBuilder = new StringBuilder();
			var last = dictionary.Last();

			foreach (var keyValuePair in dictionary)
			{
				stringBuilder.Append($"{keyValuePair.Key.UrlEncode()}={keyValuePair.Value.UrlEncode()}");

				if (keyValuePair.Key != last.Key)
				{
					stringBuilder.Append("&");
				}
			}

			httpClient.DefaultRequestHeaders.Add(name: "signature", value: stringBuilder.CreateHash());

			if (parameters.Length > 0)
			{
				var jsonStringBuilder = new StringBuilder();
				jsonStringBuilder.Append("{");

				var lastParameter = parameters.Last();

				foreach (var keyValuePair in parameters)
				{
					jsonStringBuilder.Append($"\"{keyValuePair.Key}\" : \"{keyValuePair.Value}\"");

					if (lastParameter.Key != keyValuePair.Key)
					{
						jsonStringBuilder.Append(",");
					}
				}

				jsonStringBuilder.Append("}");

				return jsonStringBuilder.ToString();
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> PaymentCancelUpdateDatabase(Guid token)
		{
			var paymentDetails = await _db.PaymentDetails.FirstOrDefaultAsync(x => x.Token == token) ?? throw new Exception("Error updating payments");

			paymentDetails.PaymentStatus = "CANCELLED";

			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<bool> AddToDatabase(PayFastNotify payFastNotify)
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == payFastNotify.email_address) ?? throw new Exception("Error adding user. Please contact us at Email:Info@SocioleeMarking.com. We will get you sorted ASAP!");

			var paymentPlan = await GetPaymentPlan(payFastNotify.amount_gross);
			var paymentDetails = new PaymentDetail()
			{
				Id = await _uniqueIds.UniqueUserId(),
				Token = Guid.Parse(payFastNotify.token),
				UserId = user.Id,
				PaymentId = payFastNotify.m_payment_id,
				PfPaymentId = payFastNotify.pf_payment_id,
				PaymentStatus = payFastNotify.payment_status,
				PaymentPackageId = paymentPlan.Id,
				StartDate = CommonFunctions.CurrentDateTime(),
				ExpiryDate = paymentPlan.Monthly ? CommonFunctions.CurrentDateTimeAddMonths(1) : CommonFunctions.CurrentDateTimeAddMonths(12),
				FrequencyMonthly = paymentPlan.Monthly
			};

			_db.PaymentDetails.Add(paymentDetails);

			await _db.SaveChangesAsync();

			return true;
		}

		private async Task<PaymentPackage> GetPaymentPlan(string amount)
		{
			var paymentPlan = await _db.PaymentPackages
								.Where(x => x.Amount == double.Parse(amount))
								.FirstOrDefaultAsync() ?? throw new Exception("Payment plan does not exist.");

			return paymentPlan;
		}

		private async Task<bool> PaymentPlanExists(string amount)
		{
			var paymentPlanExists = await _db.PaymentPackages
								.AnyAsync(x => x.Amount == double.Parse(amount));

			return paymentPlanExists;
		}

		public async Task<bool> ValidateAmountPaid(string amountGross) => await PaymentPlanExists(amountGross);

		public bool ValidatePayFastSignature(PayFastNotify payFastNotify)
		{
			var url = ToKeyValueURL(payFastNotify);

			var encodedUrl = ToKeyValueEncodedURL(url);
			var calculatedSignature = Signature(encodedUrl);

			return payFastNotify.signature == calculatedSignature;
		}

		public async Task<bool> ValidPayfastDomain(IPAddress? iPAddress)
		{
			if (iPAddress == null)
				return false;

			//"197.97.145.144/28",
			//		"(197.97.145.144 - 197.97.145.159)",
			//		"41.74.179.192/27", "(41.74.179.192 – 41.74.179.223)",
			//		"102.216.36.0/28", "(102.216.36.0 - 102.216.36.15)",
			//		"102.216.36.128/28","(102.216.36.128 - 102.216.36.143)",
			//		"144.126.193.139"
			string[] domains = { "www.payfast.co.za", "w1w.payfast.co.za",
					"w2w.payfast.co.za", "sandbox.payfast.co.za"};
			var validApiAddresses = new List<IPAddress>();
			for (int i = 0; i < domains.Length; i++)
			{
				validApiAddresses.AddRange(await Dns.GetHostAddressesAsync(domains[i]));
			}

			return validApiAddresses.Contains(iPAddress);
		}

		public async Task<bool> ValidateData(PayFastNotify payFastNotify)
		{
			try
			{
				using HttpClient client = new();
				var response = await client.PostAsJsonAsync(_configuration.GetSection("PayFastSettings:ValidateUrl").Value, payFastNotify);

				if (response.IsSuccessStatusCode)
				{
					var responseMessage = await response.Content.ReadAsStringAsync();
					if (responseMessage == "VALID")
						return true;
				}
			}
			catch
			{
				return false;
			}
			return false;
		}

		public async Task<bool> SubscriptionExpiryEmail(User user, string expiryDate)
		{
			await _registerAccountService.SubscriptionExpiryEmail(user.Id, user.FullName ?? "", user.Email, expiryDate);
			return true;
		}

		public async Task<SubscriptionPaymentDetails?> GetSubscriptionPackage(Guid userId)
		{
			var paymentDetails = await _db.PaymentDetails
					.Include(x => x.PaymentPackage)
					.Where(x => x.UserId == userId)
					.OrderByDescending(x => x.ExpiryDate)
					.Select(x => new SubscriptionPaymentDetails(x.Token, x.PaymentStatus, x.ExpiryDate, x.PaymentPackage))
					.FirstOrDefaultAsync();

			return paymentDetails;
		}

		#region Private methods
		private static string ToKeyValueURL(object obj)
		{
			var keyvalues = obj.GetType().GetProperties()
				.ToList().Where(p => p.Name != "signature")
				.Select(p => $"{p.Name}={p.GetValue(obj)?.ToString()?.Trim()}")
				.ToArray();

			return string.Join('&', keyvalues);
		}

		private static string ToKeyValueEncodedURL(string obj)
		{
			string returnValue = "";
			var split = obj.Split("&");
			foreach (var value in split)
			{
				var keyValuePair = value.Split("=");
				returnValue += $"{keyValuePair[0]}={Uri.EscapeDataString(keyValuePair[1])?.Replace("%20", "+")}&";
			}
			return returnValue.Remove(returnValue.Length - 1, 1);
		}

		private string Signature(string url)
		{
			var urlEscaped = $"{url}&passphrase={Uri.EscapeDataString(_configuration.GetValue<string>("Payfast_PassPhrase")).Trim().Replace("%20", "+")}";

			//var urlEscaped = $"{url}&passphrase={Uri.EscapeDataString(_configuration.GetSection("PayFastSettings:Payfast_PassPhrase").Value).Trim().Replace("%20", "+")}";



			return CreateMD5(urlEscaped);
		}

		private static string CreateMD5(string input)
		{
			using MD5 md5 = MD5.Create();
			byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			return Convert.ToHexString(hashValue).ToLower();
		}

		#endregion
	}
}

