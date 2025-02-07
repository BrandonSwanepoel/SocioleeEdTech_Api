using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RazorHtmlEmails.RazorClassLib.Views.Models
{
	using System.ComponentModel.DataAnnotations;  
    public class ResetPasswordRequest
    {
		[Required]

		[MinLength(8, ErrorMessage = "Password hint:Password must contain at least one number, one\n                  uppercase and a lowercase letter and at least 8 characters<br />Password\n                  cannot contain whitespace.")]
		[DataType(DataType.Password)]
		[Display(Name = "password")]
		public string Password { get; set; } = string.Empty;

		[Compare("Password", ErrorMessage = "Passwords don't match.")]
		[DataType(DataType.Password)]
		[Display(Name = "confirm password")]
		public string ConfirmPassword { get; set; } = string.Empty;
    }
}

