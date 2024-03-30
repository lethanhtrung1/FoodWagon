using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;

namespace FoodWagon.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository {
		private readonly ApplicationDbContext _dbContext;

		public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ApplicationUser applicationUser) {
			_dbContext.ApplicationUsers.Update(applicationUser);
		}
	}
}
