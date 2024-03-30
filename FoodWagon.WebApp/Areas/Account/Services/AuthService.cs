using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.WebApp.Areas.Account.Models.Dto;
using FoodWagon.WebApp.Areas.Account.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace FoodWagon.WebApp.Areas.Account.Services
{
    public class AuthService : IAuthService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AuthService(IUnitOfWork unitOfWork,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager) {
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto) {
			var user = _unitOfWork.ApplicationUser.Get(x => x.Email.ToLower() == loginRequestDto.UserName.ToLower());

			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

			if(user == null || !isValid) {
				return new ResponseDto() {
					Message = "Login fail",
					Result = new LoginResponseDto() { User = null, Token = "" }
				};
			}

			var roles = await _userManager.GetRolesAsync(user);
			// generate token
			// ...

			LoginResponseDto loginResponseDto = new() {
				User = user,
				Token = ""
			};
			return new ResponseDto {
				IsSuccess = true,
				Message = "Login successful",
				Result = loginResponseDto
			};
		}

		public async Task<ResponseDto> Register(RegisterRequestDto registerRequestDto) {
			ApplicationUser user = new() {
				UserName = registerRequestDto.Email,
				Email = registerRequestDto.Email,
				Name = registerRequestDto.Name,
				NormalizedEmail = registerRequestDto.Email.ToUpper(),
				PhoneNumber = registerRequestDto.PhoneNumber,
			};
			ResponseDto responseDto = new ResponseDto();
			try {
				var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
				if(result.Succeeded) {
					var userToReturn = _unitOfWork.ApplicationUser.Get(x => x.UserName == registerRequestDto.Email);
					responseDto.IsSuccess = true;
					responseDto.Message = "Success";
					responseDto.Result = userToReturn;
				} else {
					responseDto.IsSuccess = false;
					responseDto.Message = result.Errors.FirstOrDefault().Description;
				}
				return responseDto;
			} catch (Exception) {
				throw;
			}
		}

		public async Task<ResponseDto> AssignRole(string email, string roleName) {
			var user = _unitOfWork.ApplicationUser.Get(x => x.UserName.ToLower() == email.ToLower());

			var responseDto = new ResponseDto();
			if(user != null) {
				if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()) {
					// Create role if it does not exist
					_roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
				}
				await _userManager.AddToRoleAsync(user, roleName);
				responseDto.Message = "Success";
				return responseDto;
			}
			responseDto.IsSuccess = false;
			responseDto.Message = "Error Encountered";
			return responseDto;
		}
	}
}
