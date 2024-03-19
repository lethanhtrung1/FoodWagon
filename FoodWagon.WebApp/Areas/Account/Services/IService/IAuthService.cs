using FoodWagon.WebApp.Areas.Account.Models.Dto;

namespace FoodWagon.WebApp.Areas.Account.Services.IService {
	public interface IAuthService {
		Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
		Task<ResponseDto> Register(RegisterRequestDto registerRequestDto);
		Task<ResponseDto> AssignRole(string email, string roleName);
	}
}
