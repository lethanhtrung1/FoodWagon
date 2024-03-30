using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodWagon.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
	[Authorize]
	public class CartController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

			ShoppingCartVM = new() {
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach(var cart in ShoppingCartVM.ShoppingCarts) {
				cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.ProductId).ToList();
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				// ...
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
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
			if(cartFromDb.Count <= 1) {
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

		public IActionResult Summary() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM shoppingCartVM = new() {
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new(),
			};

			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach(var cart in shoppingCartVM.ShoppingCarts) {
				cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.ProductId).ToList();
				cart.Price = cart.Product.Price - (cart.Product.Price * cart.Product.SaleOff / 100);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(shoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult Summary(ShoppingCartVM shoppingCartVM) {
			return RedirectToAction("Index", "Home");
		}
	}
}
