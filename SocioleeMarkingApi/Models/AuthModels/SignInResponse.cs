namespace SocioleeMarkingApi.Models.AuthModels
{
	public class SignInResponse
	{
		public Guid Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public bool IsAdmin { get; set; }
		public Guid RoleId { get; set; }
		public Guid InstitutionId { get; set; }
		public string AuthenticationToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime? RefreshTokenExpires { get; set; }
		public bool NotVerified { get; set; }

		public SignInResponse(Guid id, string username,string email, bool isAdmin, Guid roleId, Guid institutionId, string authenticationToken, string refreshToken, DateTime refreshTokenExpires, bool notVerified)
		{
			Id = id;
			Username = username;
			Email = email;
			IsAdmin = isAdmin;
			RoleId = roleId;
			InstitutionId = institutionId;
			AuthenticationToken = authenticationToken;
			RefreshToken = refreshToken;
			RefreshTokenExpires = refreshTokenExpires;
			NotVerified = notVerified;
		}
	}	
}

