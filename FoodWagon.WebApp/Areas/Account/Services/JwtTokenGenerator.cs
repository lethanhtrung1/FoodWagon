using FoodWagon.Models;
using FoodWagon.WebApp.Areas.Account.Models;
using FoodWagon.WebApp.Areas.Account.Services.IService;
using Microsoft.Extensions.Options;

namespace FoodWagon.WebApp.Areas.Account.Services {
	public class JwtTokenGenerator : IJwtTokenGenerator {
		public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles) {
			throw new NotImplementedException();
		}
	}
}
