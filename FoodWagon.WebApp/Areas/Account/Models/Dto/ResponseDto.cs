namespace FoodWagon.WebApp.Areas.Account.Models.Dto {
	public class ResponseDto {
		public object? Result { get; set; }
		public string Message { get; set; } = string.Empty;
		public bool IsSuccess { get; set; } = true;
	}
}
