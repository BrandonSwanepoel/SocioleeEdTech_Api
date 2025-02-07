using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.AuthModels;

namespace SocioleeMarkingApi.Authorization;

public interface IJwtUtils
	{
		public string GenerateToken(Guid userId, string role);
		public Guid? ValidateToken(string token);
		RefreshToken GenerateRefreshToken();
	}

	public class JwtUtils : IJwtUtils
	{
		private readonly IConfiguration _configuration;

		public JwtUtils(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(Guid userId, string role)
		{
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
					new Claim(ClaimTypes.Role, role),
					new Claim("UserId", userId.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token")));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: CommonFunctions.CurrentDateTimeAddDays(365),
				signingCredentials: creds
				);
			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}

		public RefreshToken GenerateRefreshToken()
		{
			var refreshToken = new RefreshToken
			{
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				Created = CommonFunctions.CurrentDateTime(),
				Expires = CommonFunctions.CurrentDateTimeAddDays(365)
			};
			return refreshToken;
		}

		public Guid? ValidateToken(string token)
		{
			if (token == null)
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;
				var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

				// return user id from JWT token if validation successful
				return userId;
			}
			catch
			{
				// return null if validation fails
				return null;
			}
		}
	}

