using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class HomeController : Controller {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _dbContext;

		public HomeController(IUnitOfWork unitOfWork, ApplicationDbContext dbContext) {
			_unitOfWork = unitOfWork;
			_dbContext = dbContext;
		}

		public IActionResult Index() {
			DateTime StartDate = DateTime.Today.AddDays(-6);
			DateTime EndDate = DateTime.Today;

			var orders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus != SD.OrderCancelled || x.OrderStatus != SD.OrderPending).ToList();

			ViewBag.TotalOrderToday = orders.Count();
			ViewBag.TotalRevenueOrder = orders.Sum(x => x.OrderTotal).ToString("c0");

			var users = _unitOfWork.ApplicationUser.GetAll().Count();
			ViewBag.TotalUsers = users;

			var orderDetail = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product").ToList();

			ViewBag.DoughnutChart = orderDetail.GroupBy(x => x.ProductId).Select(y => new {
				product = y.First().Product.Title,
				total = y.Sum(i => i.Count * i.Price),
				formattedTotal = y.Sum(i => i.Count * i.Price).ToString("c0"),
			});

			return View();
		}
	}
}
