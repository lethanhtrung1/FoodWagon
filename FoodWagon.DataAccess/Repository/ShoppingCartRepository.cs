using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository {
		private readonly ApplicationDbContext _dbContext;

		public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ShoppingCart shoppingCart) {
			_dbContext.ShoppingCarts.Update(shoppingCart);
		}
	}
}
