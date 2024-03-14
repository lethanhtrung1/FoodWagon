﻿using FoodWagon.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodWagon.DataAccess.Data {
	public class ApplicationDbContext : DbContext {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
					new Category { Id = 1, Name = "Appetizer", DisplayOrder = 1 },
					new Category { Id = 2, Name = "Dessert", DisplayOrder = 2 },
					new Category { Id = 3, Name = "Entree", DisplayOrder = 3 }
				);
		}
	}
}
