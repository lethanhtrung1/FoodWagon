using FoodWagon.Models;

namespace FoodWagon.WebApp.Areas.Account.Models.Dto {
	public class LoginResponseDto {
		public ApplicationUser User { get; set; }
		public string Token { get; set; }
	}
}
