﻿using FoodWagon.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader> {
		void Update(OrderHeader orderHeader);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
	}
}
