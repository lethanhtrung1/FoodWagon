using System.ComponentModel.DataAnnotations;

namespace FoodWagon.WebApp.Areas.Account.Models.Dto {
	public class RegisterRequestDto {
		[Required]
		public string Email { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string ConfirmPassword { get; set; }
		public string? Role {  get; set; }
	}
}
