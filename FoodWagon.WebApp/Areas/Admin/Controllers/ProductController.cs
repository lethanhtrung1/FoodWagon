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

				HandleProductImage(productVM, files);

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

		public IActionResult Edit(int productId) {
			ProductVM productVM = new() {
				Product = _unitOfWork.Product.Get(x => x.Id == productId, includeProperties: "ProductImages"),
				CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Id.ToString()
				})
			};
			return View(productVM);
		}

		[HttpPost]
		public IActionResult Edit(ProductVM productVM, List<IFormFile> files) {
			if (ModelState.IsValid) {
				_unitOfWork.Product.Update(productVM.Product);

				HandleProductImage(productVM, files);

				_unitOfWork.Save();
				TempData["success"] = "Product updated successful.";

				return RedirectToAction("Index", "Product");
			}
			return View(productVM);
		}

		// Delete Image in Edit page
		public IActionResult DeleteImage(int imageId) {
			var imageDelete = _unitOfWork.ProductImage.Get(x => x.Id == imageId);
			int productId = imageDelete.ProductId;
			if (imageDelete != null) {
				if (!string.IsNullOrEmpty(imageDelete.ImageUrl)) {
					string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageDelete.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(imagePath)) {
						System.IO.File.Delete(imagePath);
					}
				}
				_unitOfWork.ProductImage.Remove(imageDelete);
				_unitOfWork.Save();
				TempData["success"] = "Image deleted successful.";
			}
			return RedirectToAction(nameof(Edit), new { productId = productId });
		}


		#region APIs call

		[HttpGet]
		public IActionResult GetAll() {
			try {
				IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
				//foreach (var product in products) {
				//	product.ProductImages = _unitOfWork.ProductImage.GetAll(x => x.ProductId == product.Id).ToList();
				//}
				return Json(new { data = products });
			} catch (Exception) {
				throw;
			}
		}

		[HttpDelete]
		public IActionResult Delete(int id) {
			Product productDelete = _unitOfWork.Product.Get(x => x.Id == id);
			if (productDelete == null) {
				return Json(new { success = false, message = "Error while deleting." });
			}
			string productPath = @"images\products\" + productDelete.Title;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
			if (Directory.Exists(finalPath)) {
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths) {
					System.IO.File.Delete(filePath);
				}
				Directory.Delete(finalPath);
			}
			_unitOfWork.Product.Remove(productDelete);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Deleted successful." });
		}

		#endregion

		private void HandleProductImage(ProductVM productVM, List<IFormFile> files) {
			// Hanle files image
			string wwwRootPath = _webHostEnvironment.WebRootPath;
			if (files != null) {
				foreach (IFormFile file in files) {
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = @"images\products\" + productVM.Product.Title;
					string finalPath = Path.Combine(wwwRootPath, productPath);
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
						productVM.Product.ProductImages = new List<ProductImage>();
					}
					productVM.Product.ProductImages.Add(productImage);
				}
				_unitOfWork.Product.Update(productVM.Product);
				_unitOfWork.Save();
			}
		}
	}
}
