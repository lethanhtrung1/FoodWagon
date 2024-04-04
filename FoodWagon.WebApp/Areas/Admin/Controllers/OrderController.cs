using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Details(int orderId) {
			OrderVM orderVM = new() {
				OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(orderVM);
		}


		#region APIs

		/// <summary>
		/// API Get All Order using for DataTable
		/// </summary>
		/// <param name="status"></param>
		/// <returns>json list order</returns>
		[HttpGet]
		public IActionResult GetAll(string status) {
			List<OrderHeader> orderHeaders;

			if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) {
				orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
				foreach (var order in orderHeaders) {
					string dateTime = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
					order.OrderDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
				}
			} else {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
			}

			switch (status) {
				case "pending":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderPending).ToList();
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderInProcess).ToList();
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderApproved).ToList();
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderShipped).ToList();
					break;
				default:
					break;
			}
			return Json(new { data = orderHeaders });
		}

		#endregion
	}
}
