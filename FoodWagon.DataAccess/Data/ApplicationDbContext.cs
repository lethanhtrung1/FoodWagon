﻿using FoodWagon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodWagon.DataAccess.Data {
	public class ApplicationDbContext : IdentityDbContext<IdentityUser> {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		#region DbSet

		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }

		#endregion

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
