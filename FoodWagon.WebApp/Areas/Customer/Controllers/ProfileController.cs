using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using FoodWagon.WebApp.Areas.Customer.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodWagon.WebApp.Areas.Customer.Controllers {
	[Area("Customer")]
	[Authorize]
	public class ProfileController : Controller {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IUnitOfWork _unitOfWork;

		public ProfileController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IUnitOfWork unitOfWork) {
			_userManager = userManager;
			_signInManager = signInManager;
			_unitOfWork = unitOfWork;
		}

		public async Task<IActionResult> Index() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound();
			}
			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == user.Id);
			ProfileVM profileVM = await LoadDataAsync(applicationUser);
			return View(profileVM);
		}

		[HttpPost]
		public async Task<IActionResult> Index(ProfileVM profileVM) {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			if(user == null) {
				return NotFound();
			}
			if(!ModelState.IsValid) {
				return View(profileVM);
			}

			user.PhoneNumber = profileVM.PhoneNumber;
			user.Name = profileVM.Name;
			user.StreetAddress = profileVM.StreetAddress;
			user.City = profileVM.City;
			user.Country = profileVM.Country;
			_unitOfWork.ApplicationUser.Update(user);
			_unitOfWork.Save();
			TempData["success"] = "Change profile successful.";

			await _signInManager.RefreshSignInAsync(user);
			return RedirectToAction("Index");
		}

		private async Task<ProfileVM> LoadDataAsync(ApplicationUser user) {
			ProfileVM result = new ProfileVM();
			result.UserName = await _userManager.GetUserNameAsync(user);
			result.PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
			result.Name = user.Name;
			result.StreetAddress = user.StreetAddress;
			result.City = user.City;
			result.Country = user.Country;
			return result;
		}
	}
}
