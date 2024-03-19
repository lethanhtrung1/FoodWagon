using FoodWagon.Models;
using FoodWagon.WebApp.Areas.Account.Models;
using FoodWagon.WebApp.Areas.Account.Services.IService;
using Microsoft.Extensions.Options;

namespace FoodWagon.WebApp.Areas.Account.Services {
	public class JwtTokenGenerator : IJwtTokenGenerator {
		private readonly JwtOptions _jwtOptions;

		public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions) {
			_jwtOptions = jwtOptions.Value;
		}

		public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles) {
			throw new NotImplementedException();
		}
	}
}
