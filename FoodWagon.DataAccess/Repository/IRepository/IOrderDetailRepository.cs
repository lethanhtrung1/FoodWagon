﻿using FoodWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository.IRepository {
	public interface IOrderDetailRepository : IRepository<OrderDetail> {
		void Update(OrderDetail orderDetail);
	}
}