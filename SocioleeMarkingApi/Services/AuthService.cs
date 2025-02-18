using System.Security.Cryptography;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorHtmlEmails.Common;
using SocioleeMarkingApi.Authorization;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.AuthModels;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IAuthService
    {
        Task<SignInResponse> SignIn(UserSignInRequest request);
		//Task<SignInResponse> SignInWithGoogle(UserGoogleSignInRequest request);
		Task<Guid> SignUp(UserSignUpRequest request, bool signUpWithGoogle = false);
		Task ResendEmail(string email);
		Task ResendForgotPasswordEmail(string email);
		Task<string> RefreshToken();
		Task<string> Verify(string token);
		Task<Guid> ForgotPassword(string email);
        string ResetPassword();
        Task<Guid> ChangePassword(ChangePassword changePassword);
        Task<bool> ChangePasswordVerifyLink(string token);
		Task<bool> CheckVerificationCodeEmail(UserVerificationCode userVerificationCode);
		Task<bool> CheckVerificationCodeResetPassword(UserVerificationCode userVerificationCode);
		Task<bool> CheckCouponCode(UserCouponRedeemed userCouponRedeemed);

		Task<string> GetRefreshToken(Guid userId);
	}
    public class AuthService : PageModel, IAuthService
    {
        private readonly SocioleeDesignContext _db;
        private readonly IConfiguration _configuration;
        private readonly IJwtUtils _jwtUtils;
		private readonly IRegisterAccountService _registerAccountService;
		IHttpContextAccessor _httpContextAccessor;
		private readonly IUniqueIds _uniqueIds;

		public AuthService(SocioleeDesignContext dbContext, IConfiguration configuration, IJwtUtils jwtUtils, IRegisterAccountService registerAccountService, IHttpContextAccessor httpContextAccessor, IUniqueIds uniqueIds)
        {
            _db = dbContext;
            _configuration = configuration;
            _jwtUtils = jwtUtils;
			_registerAccountService = registerAccountService;
			_httpContextAccessor = httpContextAccessor;
			_uniqueIds = uniqueIds;
		}


		public async Task<SignInResponse> SignIn(UserSignInRequest request)
        {
            var existingUser = await _db.Users
				.Include(x => x.Students)
				.Include(x => x.Role)
				.FirstOrDefaultAsync(x => x.Email == request.Email) ?? throw new HttpRequestException("Sign in details are incorrect");
			//if (existingUser.VerifiedAt == null)
			//         {
			//             throw new HttpRequestException("User email not verified");
			//         }

			Guid institutionId;
			Guid? studentId = null;

			if (existingUser.Students.Any())
			{
				var student = await _db.Students.Where(x => x.UserId == existingUser.Id).FirstOrDefaultAsync() ?? throw new HttpRequestException("Can't find your student details");

				institutionId = student.InstitutionId;
				studentId = student.Id;
			}
			else
			{
				institutionId = await _db.InstitutionLecturers.Where(x => x.UserId == existingUser.Id).Select(x => x.InstitutionId).FirstOrDefaultAsync();
			}

            if (existingUser.PasswordSalt == null || existingUser.PasswordHash == null || !VerifyPasswordHash(request.Password, existingUser.PasswordSalt, existingUser.PasswordHash))
            {
                throw new HttpRequestException("Sign in details are incorrect");
            }

			var token = _jwtUtils.GenerateToken(existingUser.Id, existingUser.Role.Name);

            var refreshToken = _jwtUtils.GenerateRefreshToken();
            SetRefreshToken(refreshToken, existingUser);

            return new SignInResponse(existingUser.Id, existingUser.FullName,existingUser.Email, existingUser.IsAdmin, existingUser.RoleId, institutionId, studentId, token, refreshToken.Token, refreshToken.Expires, existingUser.VerifiedAt == null);

			static bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
            {
				using var hmac = new HMACSHA512(passwordSalt);
				var computedHash = hmac
					.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
        }

		//public async Task<SignInResponse> SignInWithGoogle(UserGoogleSignInRequest request)
		//{
			//var settings = new GoogleJsonWebSignature.ValidationSettings()
			//{
			//	Audience = new List<string> { _configuration.GetValue<string>("GoogleClientId") }
			//};

			//var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credentials, settings);

			//var existingUser = await _db.Users
			//	.Include(x => x.Role)
			//	.FirstOrDefaultAsync(x => x.Email == payload.Email);

			//if (existingUser == null)
			//{
			//	var userDetails = new UserSignUpRequest(payload.Name, payload.Email);
			//	await SignUp(userDetails, true);
			//	existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == payload.Email);
			//}

			//Guid institutionId;


			//var token = _jwtUtils.GenerateToken(existingUser.Id, existingUser.Role.Name);

			//var refreshToken = _jwtUtils.GenerateRefreshToken();
			//SetRefreshToken(refreshToken, existingUser);

			//return new SignInResponse(existingUser.Id, existingUser.FullName, existingUser.Email, existingUser.IsAdmin, existingUser.RoleId, token, refreshToken.Token, refreshToken.Expires, false);
		//}

		private void SetRefreshToken(RefreshToken refreshToken, User user)
		{
			var cookiesOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = refreshToken.Expires,
				IsEssential = true,
				Secure = false,
				SameSite = SameSiteMode.Strict,
			};
			_httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken.Token, cookiesOptions);
			user.RefreshToken = refreshToken.Token;
			user.ResetTokenExpires = refreshToken.Expires;
			user.RefreshTokenCreated = refreshToken.Created;
			_db.SaveChangesAsync();
		}

		public async Task<string> RefreshToken()
		{
			var refreshToken = _httpContextAccessor?.HttpContext?.Request.Cookies["refreshToken"];
			var user = await _db.Users
				.Include(x => x.Role)
				.Where(x=> x.RefreshToken == refreshToken).FirstOrDefaultAsync();

            if(user == null)
			{
                throw new HttpRequestException("Sign in again");
			}
			else if(user.RefreshTokenExpires < CommonFunctions.CurrentDateTime())
			{
				throw new HttpRequestException("Sign in again");
			}

			var token = _jwtUtils.GenerateToken(user.Id, user.Role.Name);

			var newRefreshToken = _jwtUtils.GenerateRefreshToken();
			SetRefreshToken(newRefreshToken, user);
			return token;
		}


		public async Task<Guid> SignUp(UserSignUpRequest request, bool signUpWithGoogle = false)
        {
			if (_db.Users.Any(x => x.Email == request.Email))
			{
				throw new HttpRequestException("User already exists.");
			}

			var userDbModel = new User
			{
				Id = request.Id ?? await _uniqueIds.UniqueUserId() ,
				FullName = request.FullName,
				Email = request.Email,
				PasswordHash = null,
				PasswordSalt = null,
				RoleId = request.RoleId,
				VerificationToken = CommonFunctions.GenerateRandomCode(6)
			};

			if (signUpWithGoogle == false)
			{
				CreatePasswordHash(request.Password, out byte[] passwordSalt, out byte[] passwordHash);
				userDbModel.PasswordHash = passwordHash;
				userDbModel.PasswordSalt = passwordSalt;
			}
			else
			{
				userDbModel.VerifiedAt = DateTime.Now;
			}

			_db.Users.Add(userDbModel);
			await _db.SaveChangesAsync();

			if (signUpWithGoogle == false && request.Id == null) {
				await _registerAccountService.Register(request.FullName, request.Email, userDbModel.VerificationToken);
			}else if (signUpWithGoogle == false && request.Id != null)
			{
				await _registerAccountService.StudentRegister(request.FullName, request.Email, userDbModel.VerificationToken, request.InstitionName, request.Password);
			}

			return userDbModel.Id;
        }

		public async Task ResendEmail(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email) ?? throw new HttpRequestException("Error occurred with resending email");
			await _registerAccountService.Register(user.FullName, user.Email, user.VerificationToken);
		}

		public async Task ResendForgotPasswordEmail(string email)
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email) ?? throw new HttpRequestException("Error occurred with resending email");
			await _registerAccountService.ForgotPassword(user.FullName, user.Email, user.PasswordResetToken);
		}

		public async Task<string> Verify(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.VerificationToken == token) ?? throw new HttpRequestException("VerificationToken Error");
			user.VerifiedAt = CommonFunctions.CurrentDateTime();
            await _db.SaveChangesAsync();

             var str = _registerAccountService.AccountConfirmed();
            return str;
        }

        public async Task<Guid> ForgotPassword(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new HttpRequestException("User not found");
			user.PasswordResetToken = CommonFunctions.GenerateRandomCode(6);
            user.ResetTokenExpires = CommonFunctions.CurrentDateTimeAddDays(1);
            await _db.SaveChangesAsync();

            await _registerAccountService.ForgotPassword(user.FullName, user.Email, user.PasswordResetToken);

            return user.Id;
		}

		public string ResetPassword()
		{
			return _registerAccountService.ResetPassword();
		}

		
		public async Task<bool> ChangePasswordVerifyLink(string token)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
			if (user == null || user.ResetTokenExpires < CommonFunctions.CurrentDateTime())
			{
				throw new HttpRequestException("Sign in again");
			}
            return true;
		}

		public async Task<bool> CheckVerificationCodeEmail(UserVerificationCode userVerificationCode)
		{
			var user = await _db.Users.Where(x => x.Id == userVerificationCode.UserId && x.VerificationToken == userVerificationCode.Code).FirstOrDefaultAsync();

			if (user != null)
			{
				user.VerificationToken = null;
				user.VerifiedAt = CommonFunctions.CurrentDateTime();
				await _db.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<bool> CheckCouponCode(UserCouponRedeemed userCouponRedeemed)
		{
			var coupon = await _db.Coupons.Where(x => x.Code == userCouponRedeemed.Code).FirstOrDefaultAsync();
			if (coupon != null && coupon.UserId == null)
			{
				coupon.UserId = userCouponRedeemed.UserId;

				await _db.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<bool> CheckVerificationCodeResetPassword(UserVerificationCode userVerificationCode)
		{
			var user = await _db.Users.Where(x => x.Id == userVerificationCode.UserId && x.PasswordResetToken == userVerificationCode.Code).FirstOrDefaultAsync();
			
			if(user != null)
			{
				return true;
			}
			return false;
		}

		public async Task<Guid> ChangePassword(ChangePassword changePassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == changePassword.PasswordResetToken && u.Id == changePassword.UserId);
            if (user == null || user.ResetTokenExpires < CommonFunctions.CurrentDateTime())
            {
                throw new HttpRequestException("Sign in again");
            }

			CreatePasswordHash(changePassword.Password, out byte[] passwordSalt, out byte[] passwordHash);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _db.SaveChangesAsync();

            return user.Id;
        }
		public async Task<string> GetRefreshToken(Guid userId)
		{
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new HttpRequestException("Invalid user id.");

			return user.RefreshToken;
		}


		private static void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
			using var hmac = new HMACSHA512();
			passwordSalt = hmac.Key;
			passwordHash = hmac
				.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		}

        //Create check to see if the random token does not exist in the db!!!
        private string CreateRandomToken()=>
             Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
	}

}

