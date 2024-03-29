using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository {
		private readonly ApplicationDbContext _dbContext;

		public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(OrderHeader orderHeader) {
			_dbContext.OrderHeaders.Update(orderHeader);
		}
	}
}
