using System;
using System.ComponentModel.DataAnnotations;

namespace SocioleeMarkingApi.Models
{
	public class UserMailDto
	{
		public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress]
		public string Email { get; set; } = string.Empty;

		public string Subject { get; set; } = string.Empty;

		public UserMailDto(string fullName, string email, string subject)
		{
			FullName = fullName;
			Email = email;
			Subject = subject;
		}
	}
}

