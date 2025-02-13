using EllipticCurve;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Models.DTOs;
using SocioleeMarkingApi.Models.UserModels;
using SocioleeMarkingApi.Services;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
	[Authorize]
	public class UsersController : Controller
    {
        private readonly IUserService _UserService;
		//private readonly ILogger<UsersController> _logger;

		public UsersController(IUserService UserService)
        {
            _UserService = UserService;
		}

		[HttpPost("contactUs"), AllowAnonymous]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> ContactUs(ContactUsDto contactUs)
		{
			var messageSend = await _UserService.ContactUs(contactUs);
			return Ok(messageSend);
		}

		[HttpGet("checkIfEmailExists"), AllowAnonymous]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> CheckIfEmailExists([FromQuery] string email)
		{
			var emailExists = await _UserService.CheckIfEmailExists(email);
			return Ok(emailExists);
		}

		[HttpPost("addPaymentRequest")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> AddPaymentRequest([FromQuery] Guid userId, [FromQuery] int amount)
		{
			var added = await _UserService.AddPaymentRequest(userId, amount);
			return Ok(added);
		}

		//[HttpPost("alreadyAddedToWaitList"), AllowAnonymous]
		//[ProducesResponseType(typeof(bool), 200)]
		//public async Task<IActionResult> AlreadyAddedToWaitList([FromBody] WaitListDTO waitListDto)
		//{
		//	var added = await _UserService.AlreadyAddedToWaitList(waitListDto);
		//	return Ok(added);
		//}
		

		//[HttpGet("asset/{type}")]
		//[ProducesResponseType(typeof(string), 200)]
		//public async Task<IActionResult> AppDetailAsset([FromRoute] string type, [FromQuery] Guid? userId = null)
		//{
		//	var assetId = await _UserService.GetAsset(type userId);
		//	return Ok(assetId);
		//}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(UserDetails), 200)]
		public async Task<IActionResult> GetUserDetails(Guid id)
        {
            var user = await _UserService.GetUserDetails(id);
            return Ok(user);
        }

		[HttpGet("userHasCoupon/{id}")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> UserHasCoupon(Guid id)
		{
			var userHasCoupon = await _UserService.UserHasCoupon(id);
			return Ok(userHasCoupon);
		}

		[HttpGet("userDesignsLeft")]
		[ProducesResponseType(typeof(int), 200)]
		public async Task<IActionResult> UserDesignsLeft(Guid userId)
		{
			var designsLeft = await _UserService.UserDesignsLeft(userId);
			return Ok(designsLeft);
		}

		[HttpGet("userProjectCount")]
		[ProducesResponseType(typeof(int), 200)]
		public async Task<IActionResult> UserProjectCount(Guid institutionId)
		{
			var designCount = await _UserService.GetProjectCount(institutionId);
			return Ok(designCount);
		}

		[HttpGet("userStudentCount")]
		[ProducesResponseType(typeof(int), 200)]
		public async Task<IActionResult> UserStudentCount(Guid institutionId)
		{
			var designCount = await _UserService.GetStudentCount(institutionId);
			return Ok(designCount);
		}

		[HttpGet("userAnalytics")]
		[ProducesResponseType(typeof(UserAnalytics), 200)]
		public async Task<IActionResult> UserAnalytics(Guid studentId)
		{
			var analytics = await _UserService.UserAnalytics(studentId);
			return Ok(analytics);
		}

		[HttpGet("getStudentDetails")]
		[ProducesResponseType(typeof(Student), 200)]
		public async Task<IActionResult> GetStudentDetails(Guid studentUserId)
		{
			var studentDetails = await _UserService.GetStudentDetails(studentUserId);
			return Ok(studentDetails);
		}

		[HttpPost("upsertStudent")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> UpsertStudent([FromBody] UpsertStudentDTO studentDto, [FromQuery] bool editStudent = false)
		{
			var studentDetails = await _UserService.UpsertStudent(studentDto, editStudent);
			return Ok(studentDetails);
		}

		[HttpGet("Programmes")]
		[ProducesResponseType(typeof(IEnumerable<Programme>), 200)]
		public async Task<IActionResult> GetProgrammes([FromQuery] Guid institutionId)
		{
			var Programmes = await _UserService.GetProgrammes(institutionId);
			return Ok(Programmes);
		}

		[HttpDelete("deleteStudent")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> DeleteStudent([FromQuery] Guid studentId)
		{
			var deleted = await _UserService.DeleteStudent(studentId);
			return Ok(deleted);
		}

		[HttpPut("updateUserDetails")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> UpdateUserDetails(UserDto userDto)
		{
			var updatedUser = await _UserService.UpdateUserDetails(userDto);
			return Ok(updatedUser);
		}

		[HttpPut("updateUserEmail")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> UpdateUserEmail(UserDto userDto)
		{
			var updatedUser = await _UserService.UpdateUserEmail(userDto);
			return Ok(updatedUser);
		}

	}
}

