using System.ComponentModel.DataAnnotations;

namespace FoodWagon.WebApp.Areas.Account.Models.Dto {
	public class LoginRequestDto {
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
