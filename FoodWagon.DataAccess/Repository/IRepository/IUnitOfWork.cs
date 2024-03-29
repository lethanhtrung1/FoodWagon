using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository.IRepository {
	public interface IUnitOfWork {
		ICategoryRepository Category { get; }
		IProductRepository Product { get; }
		IProductImageRepository ProductImage { get; }
		IApplicationUserRepository ApplicationUser { get; }
		IShoppingCartRepository ShoppingCart { get; }
		IOrderHeaderRepository OrderHeader { get; }
		IOrderDetailRepository OrderDetail { get; }
		
		void Save();
	}
}
