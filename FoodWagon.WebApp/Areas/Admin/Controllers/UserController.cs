using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

		public IActionResult RoleManagement(string userId) {
			RoleManagementVM RoleVM = new RoleManagementVM() {
				ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId),
				RoleList = _roleManager.Roles.Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Name
				})
			};
			RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(x => x.Id == userId))
				.GetAwaiter().GetResult().FirstOrDefault()!;

			return View(RoleVM);
		}

		[HttpPost]
		public IActionResult RoleManagement(RoleManagementVM roleVM) {
			string oldRoleName = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(x => x.Id == roleVM.ApplicationUser.Id))
				.GetAwaiter().GetResult().FirstOrDefault()!;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == roleVM.ApplicationUser.Id, tracked: true);

			if (roleVM.ApplicationUser.Role != oldRoleName) {
				// remove old role
				_userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
				// add new role
				_userManager.AddToRoleAsync(applicationUser, roleVM.ApplicationUser.Role).GetAwaiter().GetResult();
				TempData["success"] = "User Role Updated Successfully";
			}
			return RedirectToAction("Index");
		}


		#region API

		[HttpGet]
		public IActionResult GetAll() {
			List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll().ToList();
			foreach (var user in users) {
				user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
			}
			return Json(new { data = users });
		}

		[HttpPost]
		public IActionResult LockUnlock([FromBody] string userId) {
			// ...
			var user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			if(user == null) {
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}
			if(user.LockoutEnd != null && user.LockoutEnd > DateTime.Now) {
				// user is currently locked and we need unlock them
				user.LockoutEnd = DateTime.Now;
			} else {
				user.LockoutEnd = DateTime.Now.AddDays(1000);
			}
			_unitOfWork.Save();

			return Json(new { success = true, message = "User Lock/Unlock successful." });
		}

		#endregion
	}
}
