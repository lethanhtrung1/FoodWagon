﻿using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class UnitOfWork : IUnitOfWork {
		private readonly ApplicationDbContext _dbContext;
		public ICategoryRepository Category { get; private set; }

		public UnitOfWork(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
			Category = new CategoryRepository(_dbContext);
		}

		public void Save() {
			_dbContext.SaveChanges();
		}
	}
}