using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	public class CategoryController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			List<Category> categories = _unitOfWork.Category.GetAll().ToList();

			return View(categories);
		}


		#region APIs Call

		[HttpGet]
		public IActionResult GetAll() {
			IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
			return Json(new { data = categories });
		}

		#endregion
	}
}
