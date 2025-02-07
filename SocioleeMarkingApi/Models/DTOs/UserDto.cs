namespace SocioleeMarkingApi.Models.UserModels
{
	public class UserDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string About{ get; set; } = string.Empty;
	}
}

