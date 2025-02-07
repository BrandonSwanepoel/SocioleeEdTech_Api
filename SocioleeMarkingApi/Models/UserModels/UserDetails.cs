using SocioleeMarkingApi.Models.BlobModel;

namespace SocioleeMarkingApi.Models
{
	public class UserDetails
	{
		public Guid Id { get; set; }
		public string? FullName { get; set; }
		public string Email { get; set; }
		public string? Password { get; set; }
		public bool IsAdmin { get; set; }
		public Guid RoleId { get; set; }

		public UserDetails(Guid id, string? fullName, string email, Guid roleId, string? password = null, bool isAdmin = false)
		{
			Id = id;
			FullName = fullName;
			Email = email;
			RoleId = roleId;
			Password = password;
			IsAdmin = isAdmin;
		}
	}
}

