using System;
using System.ComponentModel.DataAnnotations;

namespace SocioleeMarkingApi.Models
{
    public class UserGoogleSignInRequest
	{
        [Required]
        public string Credentials { get; set; } = string.Empty;
        [Required]
        public string UserType { get; set; } = string.Empty;
    }
}
