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
			DateTime EndDate = DateTime.Today.AddDays(1);

			var orders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus != SD.OrderCancelled).ToList();

			ViewBag.TotalOrderToday = orders.Count();
			ViewBag.TotalRevenueOrder = orders.Sum(x => x.OrderTotal).ToString("c0");

			var users = _unitOfWork.ApplicationUser.GetAll().Count();
			ViewBag.TotalUsers = users;

			var orderDetail = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product").ToList();

			// Doughnut Chart
			ViewBag.DoughnutChart = orderDetail.GroupBy(x => x.ProductId).Select(y => new {
				product = y.First().Product.Title,
				total = y.Sum(i => i.Count * i.Price),
				formattedTotal = y.Sum(i => i.Count * i.Price).ToString("c0"),
			});

			// Spline Chart
			var splineChartData = orders.Where(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate)
				.GroupBy(y => y.OrderDate).Select(k => new SplineChartData {
					day = k.First().OrderDate.ToString("dd-MMM"),
					total = k.Sum(i => i.OrderTotal),
				});

			string[] Last30Days = Enumerable.Range(0, 7).Select(x => StartDate.AddDays(x).ToString("dd-MMM")).ToArray();

			ViewBag.SplineChartData = from day in Last30Days
									  join data in splineChartData on day equals data.day into splineChartJoined
									  from dataChart in splineChartJoined.DefaultIfEmpty()
									  select new {
										  day = day,
										  total = dataChart == null ? 0 : dataChart.total
									  };

			//Recent Order
			ViewBag.RecentOrders = _dbContext.OrderHeaders.OrderByDescending(x => x.OrderDate).Take(5).ToList();

			return View();
		}
	}

	public class SplineChartData {
		public string day;
		public double total;
	}
}
