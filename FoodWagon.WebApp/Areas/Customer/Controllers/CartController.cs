using FoodWagon.DataAccess.Repository;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace FoodWagon.WebApp.Areas.Customer.Controllers {
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public CartController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

			ShoppingCartVM shoppingCartVM = new() {
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (var cart in shoppingCartVM.ShoppingCarts) {
				cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.ProductId).ToList();
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				// ...
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(shoppingCartVM);
		}

		public IActionResult PlusQuantityInCart(int cartId) {
			var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult MinusQuantityInCart(int cartId) {
			var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked: true);
			if (cartFromDb.Count <= 1) {
				_unitOfWork.ShoppingCart.Remove(cartFromDb);
			} else {
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult RemoveItemInCart(int cartId) {
			var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked: true);
			_unitOfWork.ShoppingCart.Remove(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		///  View Summary
		/// </summary>
		/// <returns></returns>
		public IActionResult Summary() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			CheckoutVM checkoutVM = new() {
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new(),
			};

			checkoutVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			checkoutVM.OrderHeader.PhoneNumber = checkoutVM.OrderHeader.ApplicationUser.PhoneNumber;
			checkoutVM.OrderHeader.Name = checkoutVM.OrderHeader.ApplicationUser.Name;
			checkoutVM.OrderHeader.StreetAddress = checkoutVM.OrderHeader.ApplicationUser.StreetAddress;
			checkoutVM.OrderHeader.City = checkoutVM.OrderHeader.ApplicationUser.City;

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (var cart in checkoutVM.ShoppingCarts) {
				cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.ProductId).ToList();
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				checkoutVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(checkoutVM);
		}

		/// <summary>
		///  Checkout API
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST(CheckoutVM checkoutVM) {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			checkoutVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

			checkoutVM.OrderHeader.OrderDate = DateTime.Now;
			checkoutVM.OrderHeader.ApplicationUserId = userId;

			//ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id ==  userId);

			foreach (var cart in checkoutVM.ShoppingCarts) {
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				checkoutVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if (checkoutVM.IsPaymentNow) {
				// Stripe Payment 
				checkoutVM.OrderHeader.PaymentMethod = SD.PaymentCreditCard;
				checkoutVM.OrderHeader.OrderStatus = SD.OrderApproved;
				checkoutVM.OrderHeader.PaymentStatus = SD.PaymentPending;
			} else {
				// Cash on Delivery
				checkoutVM.OrderHeader.PaymentMethod = SD.PaymentCashOnDelivery;
				checkoutVM.OrderHeader.OrderStatus = SD.OrderPending;
				checkoutVM.OrderHeader.PaymentStatus = SD.PaymentPending;
			}

			// Add Order Header to OrderDetail table
			_unitOfWork.OrderHeader.Add(checkoutVM.OrderHeader);
			_unitOfWork.Save();

			// Add Order Details to OrderDetail table
			foreach (var cart in checkoutVM.ShoppingCarts) {
				OrderDetail orderDetail = new() {
					ProductId = cart.ProductId,
					OrderHeaderId = checkoutVM.OrderHeader.Id,
					Count = cart.Count,
					Price = cart.Price,
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			// Logic Stripe
			if (checkoutVM.IsPaymentNow) {
				var domain = "https://localhost:7295/";
				var options = new SessionCreateOptions {
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={checkoutVM.OrderHeader.Id}",
					CancelUrl = domain + $"customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment"
				};

				foreach (var item in checkoutVM.ShoppingCarts) {
					var sessionLineItem = new SessionLineItemOptions {
						PriceData = new SessionLineItemPriceDataOptions {
							UnitAmount = (long)(item.Price * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions {
								Name = item.Product.Title,
							}
						},
						Quantity = item.Count,
					};
					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options); // response have a Id and PaymentIntentId

				_unitOfWork.OrderHeader.UpdateStripePaymentId(checkoutVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();

				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new {
				id = checkoutVM.OrderHeader.Id
			});
		}

		public IActionResult OrderConfirmation(int id) {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser");

			// Remove item in cart after checkout
			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			// if order is not Cash on delivery
			if (orderHeader.PaymentMethod == SD.PaymentCreditCard) {
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				// payment is successful
				if (session.PaymentStatus.ToLower() == "paid") {
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderApproved, SD.PaymentApproved);
					_unitOfWork.Save();
				}
			}

			OrderVM orderVM = new() {
				OrderHeader = orderHeader,
				OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (var cart in orderVM.OrderDetail) {
				cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.ProductId).ToList();
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				orderVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(orderVM);
		}
	}
}
