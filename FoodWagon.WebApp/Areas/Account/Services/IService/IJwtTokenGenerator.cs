using FoodWagon.Models;

namespace FoodWagon.WebApp.Areas.Account.Services.IService {
	public interface IJwtTokenGenerator {
		string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
	}
}
