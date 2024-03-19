using FoodWagon.Models;

namespace FoodWagon.DataAccess.Repository.IRepository {
	public interface IApplicationUserRepository : IRepository<ApplicationUser> {
		void Update(ApplicationUser applicationUser);
	}
}
