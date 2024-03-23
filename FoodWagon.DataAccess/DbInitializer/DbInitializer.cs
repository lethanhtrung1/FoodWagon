using FoodWagon.DataAccess.Data;
using FoodWagon.Models;
using FoodWagon.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.DataAccess.DbInitializer {
	public class DbInitializer : IDbInitializer {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _dbContext;

		public DbInitializer(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ApplicationDbContext dbContext) {
			_dbContext = dbContext;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void Initialize() {
			// migration if not applied
			try {
				if (_dbContext.Database.GetPendingMigrations().Count() > 0) {
					_dbContext.Database.Migrate();
				}
			} catch (Exception) {
				throw;
			}

			// Create role if the are not created
			if(!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult()) {
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

				// Created Admin account
				_userManager.CreateAsync(new ApplicationUser {
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					Name = "Admin",
					PhoneNumber = "0969905002",
					StreetAddress = "km 10 Nguyen Trai Ha Dong",
					City = "Ha Noi",
					Country = "Viet Nam"
				}, "admin123aA@").GetAwaiter().GetResult();

				ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
				_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
			}

			return;
		}
	}
}
