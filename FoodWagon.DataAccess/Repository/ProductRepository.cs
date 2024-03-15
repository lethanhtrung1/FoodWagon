using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;

namespace FoodWagon.DataAccess.Repository {
	public class ProductRepository : Repository<Product>, IProductRepository {
		private readonly ApplicationDbContext _dbContext;

		public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(Product product) {
			_dbContext.Update(product);
		}
	}
}
