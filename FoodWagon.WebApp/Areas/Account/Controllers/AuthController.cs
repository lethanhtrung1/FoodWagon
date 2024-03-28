using FoodWagon.Models;
using FoodWagon.Utility;
using FoodWagon.WebApp.Areas.Account.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FoodWagon.WebApp.Areas.Account.Controllers {
	[Area("Account")]
	public class AuthController : Controller {
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;

		public AuthController(SignInManager<ApplicationUser> signInManager,
			RoleManager<IdentityRole> roleManager,
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore) {
			_signInManager = signInManager;
			_roleManager = roleManager;
			_userManager = userManager;
			_userStore = userStore;
		}

		public async Task<IActionResult> Login() {
			LoginVM loginVM = new LoginVM();
			// Clear the existing external cookie
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM loginVM) {
			if (ModelState.IsValid) {
				var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded) {
					TempData["success"] = "User logged in.";
					return RedirectToAction("Index", "Home", new {area = "Customer"});
				}
				if (result.RequiresTwoFactor) {
					// ...
				}
				if (result.IsLockedOut) {
					TempData["error"] = "User account locked out.";
					return View(loginVM);
				} else {
					TempData["error"] = "Login fail.";
					return View(loginVM);
				}
			}
			return View(loginVM);
		}

		public async Task<IActionResult> Register() {
			RegisterVM registerVM = new() {
				RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem {
					Text = i,
					Value = i
				}),
			};
			return View(registerVM);
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM registerVM) {
			if(ModelState.IsValid) {
				var user = CreateUser();

				await _userStore.SetUserNameAsync(user, registerVM.Email, CancellationToken.None);
				
				user.PhoneNumber = registerVM.PhoneNumber;
				// user.Name = registerVM.Email;
				user.Email = registerVM.Email;

				// Create user
				var result = await _userManager.CreateAsync(user, registerVM.Password);
				if (result.Succeeded) {
					TempData["success"] = "User created a new account with password.";

					// set role
					if(!string.IsNullOrEmpty(registerVM.Role)) {
						await _userManager.AddToRoleAsync(user, registerVM.Role);
					} else {
						await _userManager.AddToRoleAsync(user, SD.Role_Customer);
					}

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					
					if(_userManager.Options.SignIn.RequireConfirmedAccount) {
						// ...
					} else {
						if(User.IsInRole(SD.Role_Admin)) {
							TempData["success"] = "New User Created Successfully!";
						} else {
							await _signInManager.SignInAsync(user, isPersistent: false);
						}
						return RedirectToAction("Login", "Auth");
					}
				}
			}
			return View(registerVM);
		}

		private ApplicationUser CreateUser() {
			try {
				return Activator.CreateInstance<ApplicationUser>();
			} catch (Exception) {

				throw;
			}
		}

		[Authorize]
		public IActionResult Logout() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LogoutAPI() {
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Auth");
		}
	}
}
