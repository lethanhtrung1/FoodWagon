using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class UnitOfWork : IUnitOfWork {
		private readonly ApplicationDbContext _dbContext;
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public IProductImageRepository ProductImage { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }

		public UnitOfWork(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
			Category = new CategoryRepository(_dbContext);
			Product = new ProductRepository(_dbContext);
			ProductImage = new ProductImageRepository(_dbContext);
			ApplicationUser = new ApplicationUserRepository(_dbContext);
			ShoppingCart = new ShoppingCartRepository(_dbContext);
			OrderHeader = new OrderHeaderRepository(_dbContext);
			OrderDetail = new OrderDetailRepository(_dbContext);
		}

		public void Save() {
			_dbContext.SaveChanges();
		}
	}
}
