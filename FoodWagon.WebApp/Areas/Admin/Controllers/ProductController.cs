using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	public class ProductController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public ProductController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return View(products);
		}
	}
}
