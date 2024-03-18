using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using FoodWagon.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodWagon.WebApp.Areas.Customer.Controllers {
	[Area("Customer")]
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) {
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
			return View(productList);
		}

		public IActionResult Details(int productId) {
			Product product = _unitOfWork.Product.Get(x => x.Id == productId, includeProperties: "Category,ProductImages");
			return View(product);
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
