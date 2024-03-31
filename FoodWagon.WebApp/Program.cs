using FoodWagon.DataAccess.Data;
using FoodWagon.DataAccess.DbInitializer;
using FoodWagon.DataAccess.Repository;
using FoodWagon.DataAccess.Repository.IRepository;
using FoodWagon.Models.Models;
using FoodWagon.Utility.Utility;
using FoodWagon.WebApp.Areas.Account.Services;
using FoodWagon.WebApp.Areas.Account.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager<SignInManager<ApplicationUser>>()
	.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
	options.LoginPath = $"/Account/Auth/Login";
	options.AccessDeniedPath = $"/Account/Auth/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromHours(5);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddControllers().AddJsonOptions(x => {
	x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

SeedDatabase();

app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

// Apply migration
void SeedDatabase() {
	using(var scope = app.Services.CreateScope()) {
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}