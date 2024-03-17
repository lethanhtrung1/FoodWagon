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
			var productFromDb = _dbContext.Products.FirstOrDefault(x => x.Id == product.Id);
			if(productFromDb != null) {
				productFromDb.Title = product.Title;
				productFromDb.Description = product.Description;
				productFromDb.CategoryId = product.CategoryId;
				productFromDb.Price = product.Price;
				// EF core automatically update ProductImages table
				productFromDb.ProductImages = product.ProductImages;
			}
		}
	}
}
