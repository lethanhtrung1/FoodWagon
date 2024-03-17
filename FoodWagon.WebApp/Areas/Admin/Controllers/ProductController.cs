using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using FoodWagon.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	public class ProductController : Controller {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) {
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Create() {
			ProductVM productVM = new() {
				Product = new Product(),
				CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Id.ToString()
				})
			};
			return View(productVM);
		}

		[HttpPost]
		public IActionResult Create(ProductVM productVM, List<IFormFile> files) {
			if (ModelState.IsValid) {
				_unitOfWork.Product.Add(productVM.Product);

				// Hanle files image
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null) {
					foreach (IFormFile file in files) {
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name);
						string productPath = @"images\products\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, fileName);
						if (!Directory.Exists(finalPath)) {
							Directory.CreateDirectory(finalPath);
						}
						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {
							file.CopyTo(fileStream);
						}
						ProductImage productImage = new() {
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = productVM.Product.Id,
						};
						if (productVM.Product.ProductImages == null) {
							productImage.Product.ProductImages = new List<ProductImage>();
						}
						productVM.Product.ProductImages.Add(productImage);
					}
					_unitOfWork.Product.Update(productVM.Product);
				}

				_unitOfWork.Save();
				TempData["success"] = "Product created successful.";

				return RedirectToAction("Index", "Product");
			} else {
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Id.ToString()
				});
				return View(productVM);
			}
		}

		#region APIs call

		[HttpGet]
		public IActionResult GetAll() {
			IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category");
			return Json(new { data = products });
		}

		#endregion
	}
}
