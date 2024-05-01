using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodWagon.WebApp.Areas.Customer.Controllers {
	[Area("Customer")]
	[Authorize]
	public class OrderController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			var claimIdentity = (ClaimsIdentity)User.Identity!;
			//var user = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader
				.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();

			return View(orderHeaders);
		}

		public IActionResult Details(int orderId) {
			OrderVM orderVM = new() {
				OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties: "Product")
			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (var detail in orderVM.OrderDetail) {
				detail.Product.ProductImages = productImages.Where(x => x.ProductId == detail.ProductId).ToList();
			}

			return View(orderVM);
		}

		public IActionResult ReceiveOrder(OrderVM orderVM) {
			_unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.OrderCompleted);
			_unitOfWork.Save();
			TempData["success"] = "Order Detail updated successful.";

			return RedirectToAction(nameof(Index));
		}
	}
}
