using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository {
		private readonly ApplicationDbContext _dbContext;

		public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(OrderDetail orderDetail) {
			_dbContext.OrderDetails.Update(orderDetail);
		}
	}
}
