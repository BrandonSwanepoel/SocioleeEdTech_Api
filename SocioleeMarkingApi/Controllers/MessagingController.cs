using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessagingController : Controller
	{
		private readonly IMessagingService _MessagingService;
		private readonly IEmailTrackingService _emailTrackingService;
		//private readonly ILogger<UsersController> _logger;

		public MessagingController(IMessagingService messagingService, IEmailTrackingService emailTrackingService)
		{
			_MessagingService = messagingService;
			_emailTrackingService = emailTrackingService;
		}

		[HttpGet()]
		public async Task<IActionResult> GetMessages([FromQuery]Guid requestId)
		{
			var messages = await _MessagingService.GetMessages(requestId);
			return Ok(messages);
		}

		[HttpPost("sendMessage")]
		public async Task<IActionResult> GetMessages(MessageIn message)
		{
			var messageOut = await _MessagingService.SendMessage(message);
			return Ok(messageOut);
		}

		[HttpDelete()]
		public async Task<IActionResult> DeleteMessages([FromQuery] Guid messageId)
		{
			var deleted = await _MessagingService.DeleteMessage(messageId);
			return Ok(deleted);
		}
	}
}

