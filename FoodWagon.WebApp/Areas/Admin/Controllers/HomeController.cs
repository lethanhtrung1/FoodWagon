using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	public class HomeController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}
