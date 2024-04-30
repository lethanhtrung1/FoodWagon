using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Globalization;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _dbContext;

		public OrderController(IUnitOfWork unitOfWork, ApplicationDbContext dbContext) {
			_unitOfWork = unitOfWork;
			_dbContext = dbContext;
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

		// Update Order Detail
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult UpdateOrderDetail(OrderVM orderVM) {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);

			orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = orderVM.OrderHeader.City;
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier)) {
				orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber)) {
				orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["success"] = "Order Detail updated successful.";

			return RedirectToAction(nameof(Details), new {
				orderId = orderHeaderFromDb.Id,
			});
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcessing(OrderVM orderVM) {
			_unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.OrderInProcess);
			_unitOfWork.Save();
			TempData["success"] = "Order Detail updated successful.";

			return RedirectToAction(nameof(Details), new {
				orderId = orderVM.OrderHeader.Id
			});
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult ShipOrder(OrderVM orderVM) {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);

			orderHeader.Carrier = orderVM.OrderHeader.Carrier;
			orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			orderHeader.OrderStatus = SD.OrderShipped;
			orderHeader.ShippingDate = DateTime.Now;

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["success"] = "Order shipped successfully";

			return RedirectToAction(nameof(Details), new {
				orderId = orderVM.OrderHeader.Id
			});
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult CancelOrder(OrderVM orderVM) {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);

			if (orderHeader.PaymentStatus == SD.PaymentApproved) {
				var options = new RefundCreateOptions {
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};
				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderCancelled, SD.OrderRefunded);
			} else {
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderCancelled, SD.OrderCancelled);
			}
			_unitOfWork.Save();
			TempData["success"] = "Order Cancelled successfully";

			return RedirectToAction(nameof(Details), new {
				orderId = orderHeader.Id
			});
		}


		#region APIs

		/// <summary>
		///  Get Orders
		///  Handle server side datatables
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult GetAll(string status) {
			try {
				// var req = Request.Form;

				var currentPage = Request.Form["draw"].FirstOrDefault();
				var start = Request.Form["start"].FirstOrDefault();
				var length = Request.Form["length"].FirstOrDefault();

				var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
				var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
				var searchValue = Request.Form["search[value]"].FirstOrDefault();

				int pageSize = length != null ? Convert.ToInt32(length) : 0;
				int skip = start != null ? Convert.ToInt32(start) : 0;

				var listData = _dbContext.OrderHeaders.Include(x => x.ApplicationUser).ToList();
				var totalRecord = listData.Count();

				if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir)) {
					if (sortColumnDir == "asc") {
						listData = listData.OrderBy(x => x.OrderDate).ToList();
					} else if (sortColumnDir == "desc") {
						listData = listData.OrderByDescending(x => x.OrderDate).ToList();
					}
				}
				if (!string.IsNullOrEmpty(searchValue)) {
					listData = listData.Where(x => x.ApplicationUser.Name.Contains(searchValue) || x.OrderStatus.Contains(searchValue)).ToList();
				}

				switch (status) {
					case "pending":
						listData = listData.Where(x => x.OrderStatus == SD.OrderPending).Skip(skip).Take(pageSize).ToList();
						break;
					case "inprocess":
						listData = listData.Where(x => x.OrderStatus == SD.OrderInProcess).Skip(skip).Take(pageSize).ToList();
						break;
					case "approved":
						listData = listData.Where(x => x.OrderStatus == SD.OrderApproved).Skip(skip).Take(pageSize).ToList();
						break;
					case "completed":
						listData = listData.Where(x => x.OrderStatus == SD.OrderShipped).Skip(skip).Take(pageSize).ToList();
						break;
					default:
						// Get All
						listData = listData.Skip(skip).Take(pageSize).ToList();
						break;
				}
				foreach (var order in listData) {
					string dateTime = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
					order.OrderDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
				}

				var jsonResult = new {
					draw = currentPage,
					recordsFiltered = totalRecord,
					recordsTotle = totalRecord,
					data = listData
				};

				return new JsonResult(jsonResult);
			} catch (Exception) {

				throw;
			}
		}

		#endregion
	}
}
