using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FoodWagon.WebApp.Areas.Account.Models.ViewModels {
	public class RegisterVM {
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string? Role { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> RoleList { get; set; }

		//public IList<AuthenticationScheme> ExternalLogins {  get; set; }

		//public string? ReturnUrl { get; set; } = string.Empty;
	}
}
