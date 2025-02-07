using System.ComponentModel.DataAnnotations;

namespace SocioleeMarkingApi.Models
{
    public class UserSignUpBusinessRequest
	{
		[Required]
		public Guid? BusinessId { get; set; }
		[Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
	}
}

