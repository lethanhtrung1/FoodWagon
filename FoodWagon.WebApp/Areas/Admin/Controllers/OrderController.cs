using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

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


		#region APIs

		//[HttpGet]
		//public IActionResult GetAll(string status) {
		//	List<OrderHeader> orderHeaders;

		//	if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) {
		//		orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
		//		foreach (var order in orderHeaders) {
		//			string dateTime = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
		//			order.OrderDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		//		}
		//	} else {
		//		var claimsIdentity = (ClaimsIdentity)User.Identity;
		//		var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
		//		orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
		//	}

		//	switch (status) {
		//		case "pending":
		//			orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderPending).ToList();
		//			break;
		//		case "inprocess":
		//			orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderInProcess).ToList();
		//			break;
		//		case "approved":
		//			orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderApproved).ToList();
		//			break;
		//		case "completed":
		//			orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.OrderShipped).ToList();
		//			break;
		//		default:
		//			break;
		//	}
		//	return Json(new { data = orderHeaders });
		//}

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
