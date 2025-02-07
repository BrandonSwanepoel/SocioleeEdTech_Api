using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Models.AuthModels;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Services;
using RazorHtmlEmails.RazorClassLib.Views.Models;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
		private readonly IAuthService _AuthService;
		private readonly IEmailTrackingService _emailTrackingService;
		//private readonly ILogger<UsersController> _logger;

		public AuthController(IAuthService authService, IEmailTrackingService emailTrackingService)
        {
            _AuthService = authService;
			_emailTrackingService = emailTrackingService;
		}

        [HttpPost("signIn")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(Task<SignInResponse>), 200)]
		public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request)
        {
            var signInResponse = await _AuthService.SignIn(request);

			return Ok(signInResponse);
        }

		//[HttpPost("signInWithGoogle")]
		//public async Task<IActionResult> SignInWithGoogle([FromBody] UserGoogleSignInRequest request)
		//{
		//	var signInResponse = await _AuthService.SignInWithGoogle(request);
			
		//	return Ok(signInResponse);
		//}

		[HttpPost("signUp")]
		[AllowAnonymous]
        public async Task<IActionResult> SignUp(UserSignUpRequest request)
        {
            var userId = await _AuthService.SignUp(request);
			if (userId != Guid.Empty)
			{
				await _emailTrackingService.AddToEmailTracking(userId: userId);
			}

			return Ok(userId);
        }

		[HttpPost("resendEmail")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> ResendEmail([FromQuery] string email)
		{
			await _AuthService.ResendEmail(email);
			return Ok(true);
		}

		[HttpPost("ResendForgotPasswordEmail")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> ResendForgotPasswordEmail([FromQuery] string email)
		{
			await _AuthService.ResendForgotPasswordEmail(email);
			return Ok(true);
		}

		[HttpGet("refreshToken")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(Task<string>), 200)]
		public async Task<IActionResult> RefreshToken()
		{
			var token = await _AuthService.RefreshToken();
			return Ok(token);
		}

        [HttpGet("forgotPassword/{email}")]
		[ProducesResponseType(typeof(Task), 200)]
		public async Task<IActionResult> ForgotPassword(string email)
        {
            var users = await _AuthService.ForgotPassword(email);
            return Ok(users);
        }

		[HttpPost("changePassword")]
		public async Task<IActionResult> ChangePasswordView(ChangePassword changePassword)
		{
			var userId = await _AuthService.ChangePassword(changePassword);
			return Ok(userId);
		}

		[HttpPost("checkVerificationCode")]
		public async Task<IActionResult> CheckVerificationCode([FromBody] UserVerificationCode userVerificationCode)
		{
			bool valid;
			if (userVerificationCode.ResetPassword)
			{
				valid = await _AuthService.CheckVerificationCodeResetPassword(userVerificationCode);
			}
			else
			{
				valid = await _AuthService.CheckVerificationCodeEmail(userVerificationCode);
			}
			return Ok(valid);
		}

		[HttpPost("checkCouponCode")]
		public async Task<IActionResult> CheckCouponCode([FromBody] UserCouponRedeemed userCouponRedeemed)
		{
			bool valid = await _AuthService.CheckCouponCode(userCouponRedeemed);
			return Ok(valid);
		}
	}
}

