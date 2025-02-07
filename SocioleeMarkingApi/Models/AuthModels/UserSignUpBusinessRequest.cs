using System.ComponentModel.DataAnnotations;

namespace SocioleeMarkingApi.Models
{
    public class UserSignUpRequest
    {

        public UserSignUpRequest(string fullName, string email) {
            FullName = fullName;
            Email = email;
        }

		public UserSignUpRequest(Guid? id, string? institionName, string fullName, string email,string password)
		{
            Id = id;
            InstitionName = institionName;
			FullName = fullName;
			Email = email;
            Password = password;
		}


		public Guid? Id { get; set; }
		public string? InstitionName { get; set; }
		[Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
	}
}

