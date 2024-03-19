using FoodWagon.Utility;
using FoodWagon.WebApp.Areas.Account.Models.Dto;
using FoodWagon.WebApp.Areas.Account.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace FoodWagon.WebApp.Areas.Account.Controllers {
	[Area("Account")]
	public class AuthController : Controller {
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService) {
			_authService = authService;
		}

		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginRequestDto obj) {
			ResponseDto responseDto = await _authService.Login(obj);
			if(responseDto != null && responseDto.IsSuccess) {
				// ...
				LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
				//await SignInUser(loginResponseDto);

				return RedirectToAction("Index", "Home");
			} else {
				TempData["error"] = responseDto.Message;
				return View(obj);
			}
		}

		public IActionResult Register() {
			var roleList = new List<SelectListItem> {
				new SelectListItem { Text = SD.Role_Admin, Value = SD.Role_Admin },
				new SelectListItem { Text = SD.Role_Employee, Value = SD.Role_Employee },
				new SelectListItem { Text = SD.Role_Customer, Value = SD.Role_Customer }
			};
			return View(roleList);
		}
	}
}
