using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodWagon.WebApp.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IUnitOfWork _unitOfWork;

		public UserController(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork) {
			_userManager = userManager;
			_roleManager = roleManager;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			return View();
		}

		#region API

		[HttpGet]
		public IActionResult GetAll() {
			List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll().ToList();
			foreach(var user in users) {
				user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
			}
			return Json(new { data = users });
		}

		#endregion
	}
}
