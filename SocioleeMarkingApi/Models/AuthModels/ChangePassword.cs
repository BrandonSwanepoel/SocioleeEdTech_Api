namespace SocioleeMarkingApi.Models.AuthModels
{
	public class ChangePassword
	{
		public Guid UserId { get; set; }
		public string PasswordResetToken { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}

