﻿using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.Repository {
	public class Repository<T> : IRepository<T> where T : class {
		private readonly ApplicationDbContext _context;
		internal DbSet<T> dbSet;

		public Repository(ApplicationDbContext context) {
			_context = context;
			this.dbSet = _context.Set<T>(); // dbSet == _dbContext.Categories
		}

		public void Add(T entity) {
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false) {
			IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();
			query = query.Where(filter);
			if (!string.IsNullOrEmpty(includeProperties)) {
				foreach (var property in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(property);
				}
			}
			return query.FirstOrDefault();
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null) {
			IQueryable<T> query = dbSet;
			if (filter != null) {
				query = query.Where(filter);
			}

			if (!string.IsNullOrEmpty(includeProperties)) {
				foreach (var includeProperty in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(includeProperty);
				}
			}

			return query.ToList();
		}

		public void Remove(T entity) {
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities) {
			dbSet.RemoveRange(entities);
		}
	}
}
