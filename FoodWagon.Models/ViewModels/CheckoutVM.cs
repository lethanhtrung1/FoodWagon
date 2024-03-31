using FoodWagon.Models.Models;

namespace FoodWagon.Models.ViewModels {
	public class CheckoutVM {
		public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
		public OrderHeader OrderHeader { get; set; }
		public bool IsPaymentNow { get; set; } = true;
	}
}
