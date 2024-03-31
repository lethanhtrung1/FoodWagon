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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository {
		private readonly ApplicationDbContext _dbContext;

		public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(OrderHeader orderHeader) {
			_dbContext.OrderHeaders.Update(orderHeader);
		}

		/// <summary>
		/// Update Order Status
		/// </summary>
		/// <param name="id"></param>
		/// <param name="orderStatus"></param>
		/// <param name="paymentStatus"></param>
		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null) {
			var orderFromDB = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderFromDB != null) {
				orderFromDB.OrderStatus = orderStatus;
				if(!string.IsNullOrEmpty(paymentStatus)) {
					orderFromDB.PaymentStatus = paymentStatus;
				}
			}
		}

		/// <summary>
		/// Update Order Session Id & Payment Intent Id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="sessionId"></param>
		/// <param name="paymentIntentId"></param>
		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId) {
			var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderFromDb != null && !string.IsNullOrEmpty(sessionId)) {
				orderFromDb.SessionId = sessionId;
			}
			if(orderFromDb != null && !string.IsNullOrEmpty(paymentIntentId)) {
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.OrderDate = DateTime.Now;
			}
		}
	}
}
