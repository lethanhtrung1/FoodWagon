using System.ComponentModel.DataAnnotations;

namespace FoodWagon.WebApp.Areas.Customer.Models.ViewModels {
	public class EmailVM {
		public string Email { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "New email")]
		public string NewEmail { get; set; }
	}
}
