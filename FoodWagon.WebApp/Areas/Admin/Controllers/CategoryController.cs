using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
	public class CategoryController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			List<Category> categories = _unitOfWork.Category.GetAll().OrderBy(x => x.DisplayOrder).ToList();
			return View(categories);
		}

		public IActionResult Create() {
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category category) {
			if(ModelState.IsValid) {
				_unitOfWork.Category.Add(category);
				_unitOfWork.Save();
				TempData["success"] = "Category created successful";
				return RedirectToAction("Index", "Category");
			}
			return View();
		}

		public IActionResult Edit(int? categoryId) {
			if(categoryId == null || categoryId == 0) {
				return NotFound();
			}
			Category? category = _unitOfWork.Category.Get(x => x.Id == categoryId);
			if(category == null) {
				return NotFound();
			}
			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category category) {
			if(ModelState.IsValid) {
				_unitOfWork.Category.Update(category);
				_unitOfWork.Save();
				TempData["success"] = "Category updated successful";
				return RedirectToAction("Index", "Category");
			}
			return View(category);
		}

		#region APIs Call

		[HttpGet]
		public IActionResult GetAll() {
			IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
			return Json(new { data = categories });
		}

		[HttpDelete]
		public IActionResult Delete(int? id) {
			Category categoryToBeDelete = _unitOfWork.Category.Get(x => x.Id == id);
			if(categoryToBeDelete == null) {
				return Json(new { success = false, message = "Error while delete." });
			}
			_unitOfWork.Category.Remove(categoryToBeDelete);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Deleted successfully." });
		}

		#endregion
	}
}
