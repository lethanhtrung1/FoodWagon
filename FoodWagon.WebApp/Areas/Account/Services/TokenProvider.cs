using FoodWagon.Utility;
using FoodWagon.WebApp.Areas.Account.Services.IService;

namespace FoodWagon.WebApp.Areas.Account.Services {
	public class TokenProvider : ITokenProvider {
		private readonly IHttpContextAccessor _contextAccessor;

		public TokenProvider(IHttpContextAccessor httpContextAccessor) {
			_contextAccessor = httpContextAccessor;
		}

		public void ClearToken() {
			_contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
		}

		public string? GetToken() {
			string? token = null;

			bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);

			return hasToken is true ? token : null;
		}

		public void SetToken(string token) {
			_contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
		}
	}
}
