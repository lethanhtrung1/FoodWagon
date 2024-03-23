using System.ComponentModel.DataAnnotations;

namespace FoodWagon.WebApp.Areas.Customer.Models.ViewModels {
	public class ProfileVM {
		public string UserName { get; set; }
		[Phone]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }
		public string? Name { get; set; }
		[Display(Name = "Street address")]
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
	}
}
