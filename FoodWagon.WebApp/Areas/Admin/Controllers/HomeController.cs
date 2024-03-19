using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class HomeController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}
