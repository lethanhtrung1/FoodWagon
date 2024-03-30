using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository {
		private readonly ApplicationDbContext _dbContext;

		public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ProductImage productImage) {
			_dbContext.Update(productImage);
		}
	}
}
