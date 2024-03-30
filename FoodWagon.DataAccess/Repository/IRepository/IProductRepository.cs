using FoodWagon.Models.Models;

namespace FoodWagon.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product> {
		void Update(Product product);
	}
}
