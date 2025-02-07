using Microsoft.AspNetCore.Mvc;
using RazorHtmlEmails.Common;
using RazorHtmlEmails.RazorClassLib.Views.Models;
using SocioleeMarkingApi.Enums.EmailTracking;
using SocioleeMarkingApi.Services;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailTrackingController : Controller
    {
        private readonly IEmailTrackingService _emailTrackingService;
		private readonly IRegisterAccountService _registerAccountService;

		public EmailTrackingController(IEmailTrackingService emailTrackingService, IRegisterAccountService registerAccountService)
        {
			_registerAccountService = registerAccountService;
			_emailTrackingService = emailTrackingService;
        }

		[HttpGet("unsubscribeMarketing/{userId}")]
		public async Task<IActionResult> UnsubscribeMarketing(Guid userId)
        {
			var unSubscribed = await _emailTrackingService.Unsubscribe(userId, EmailTrackingType.Marketing);
			if(unSubscribed == true)
			{
				var subscriptionTypeModel = new SubscriptionTypeModel(EmailTrackingType.Marketing.ToString());
				return View(_registerAccountService.UnsubscribedPage(), subscriptionTypeModel);
			}
			return Ok();
		}

		[HttpGet("unsubscribeNewReview/{userId}")]
		public async Task<IActionResult> UnsubscribeNewReview(Guid userId)
		{
			var unSubscribed = await _emailTrackingService.Unsubscribe(userId, EmailTrackingType.Review);
			if (unSubscribed == true)
			{
				var subscriptionTypeModel = new SubscriptionTypeModel(EmailTrackingType.Review.ToString());
				return View(_registerAccountService.UnsubscribedPage(), subscriptionTypeModel);
			}
			return Ok();
		}

		[HttpGet("unsubscribeNewMessage/{userId}")]
		public async Task<IActionResult> UnsubscribeNewMessage(Guid userId)
		{
			var unSubscribed = await _emailTrackingService.Unsubscribe(userId, EmailTrackingType.Message);
			if (unSubscribed == true)
			{
				var subscriptionTypeModel = new SubscriptionTypeModel(EmailTrackingType.Message.ToString());
				return View(_registerAccountService.UnsubscribedPage(), subscriptionTypeModel);
			}
			return Ok();
		}

		[HttpPost("subscribeMarketing/{userId}")]
		public async Task<IActionResult> Subscribe(Guid userId)
		{
			var subscribed = await _emailTrackingService.Subscribe(userId, EmailTrackingType.Marketing);
			return Ok(subscribed);
		}

		[HttpPost("subscribeNewReview/{userId}")]
		public async Task<IActionResult> SubscribeNewReview(Guid userId)
		{
			var subscribed = await _emailTrackingService.Subscribe(userId, EmailTrackingType.Review);
			return Ok(subscribed);
		}

		[HttpPost("subscribeNewMessage/{userId}")]
		public async Task<IActionResult> SubscribeNewMessage(Guid userId)
		{
			var subscribed = await _emailTrackingService.Subscribe(userId, EmailTrackingType.Message);
			return Ok(subscribed);
		}
	}
}

