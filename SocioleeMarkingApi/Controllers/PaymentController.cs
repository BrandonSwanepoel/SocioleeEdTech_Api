using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayFast;
using SocioleeMarkingApi.Models.PaymentModels;
using SocioleeMarkingApi.Services;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubscriptionController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IPaymentService _paymentService;

		public SubscriptionController(IConfiguration configuration, IPaymentService paymentService)
		{
			_configuration = configuration;
			_paymentService = paymentService;
		}

		[AllowAnonymous]
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] SubscriptionUserDetails request)
		{
			var url = await _paymentService.CreateSubscriptionRequestUrl(request);
			
			var redirectUrl = $"{_configuration.GetSection("PayFastSettings:ProcessUrl").Value}{url}";

			return Ok(new PayfastProcessUrl { Url = redirectUrl });
		}

		[HttpPost("cancel")]
		public async Task<IActionResult> Cancel([FromBody] CancelUserdetails userdetails)
		{
			var cancelled = await _paymentService.Cancel(userdetails);
			return Ok(cancelled);
		}

		[HttpGet("details/{userId}")]
		public async Task<IActionResult> Details(Guid userId)
		{
			var details = await _paymentService.Details(userId);
			return Ok(details);
		}

		[AllowAnonymous]
		[HttpPost("notify")]
		public async Task<IActionResult> Notify([ModelBinder(typeof(Payments.PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
		{
			if (
				!_paymentService.ValidatePayFastSignature(payFastNotifyViewModel) ||
			await _paymentService.ValidPayfastDomain(HttpContext.Connection.RemoteIpAddress) == false ||
			!await _paymentService.ValidateAmountPaid(payFastNotifyViewModel.amount_gross) ||
			await _paymentService.ValidateData(payFastNotifyViewModel) == false)
			{
				return StatusCode(500, "Internal server error");
			}

			await _paymentService.AddToDatabase(payFastNotifyViewModel);
			return Ok();
		}

		[HttpPost("userSubscriptionExpiryEmail/{expiryDate}")]
		public async Task<IActionResult> UserSubscriptionExpiryEmail([FromBody] User user, string expiryDate)
		{
			var emailSent = await _paymentService.SubscriptionExpiryEmail(user, expiryDate);
			return Ok(emailSent);
		}

		[HttpGet("activeSubscription/{userId}")]
		public async Task<IActionResult> ActiveSubscription(Guid userId)
		{
			var cancelled = await _paymentService.ActiveSubscription(userId);
			return Ok(cancelled);
		}

		[HttpGet("hasPremiumSubscription/{userId}")]
		public async Task<IActionResult> HasPremiumSubscription(Guid userId)
		{
			var cancelled = await _paymentService.HasPremiumSubscription(userId);
			return Ok(cancelled);
		}

		[HttpGet("getSubscriptionTypeDetails/{userId}")]
		public async Task<IActionResult> GetSubscriptionTypeDetails(Guid userId)
		{
			var subscriptionTypeDetails = await _paymentService.GetSubscriptionTypeDetails(userId);
			return Ok(subscriptionTypeDetails);
		}

		[HttpGet("getSubscriptionPackage")]
		public async Task<IActionResult> GetSubscriptionPackage([FromQuery] Guid userId)
		{
			var subscriptionPackage = await _paymentService.GetSubscriptionPackage(userId);
			return Ok(subscriptionPackage);
		}

		[HttpGet("cancelledSubscription/{userId}")]
		public async Task<IActionResult> CancelledSubscription(Guid userId)
		{
			var cancelled = await _paymentService.CancelledSubscription(userId);
			return Ok(cancelled);
		}
	}
}
