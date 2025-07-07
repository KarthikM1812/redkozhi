using ChickenWeb.Data;
using ChickenWeb.DataAccess.Repository;
using ChickenWeb.Domain.Interfaces.IHome;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChickenWeb.Service.Service;
using ChickenWeb.Domain.Entities;
using ChickenWeb.Domain.Interfaces.IAdmin;
using ChickenWeb.Domain.Interfaces.IAccount;

var builder = WebApplication.CreateBuilder(args);

// Configure SQL Server DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Setup Identity (for user login/register)
builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add session support for admin login
builder.Services.AddSession();

// Add MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity UI

// ✅ Register your services BEFORE builder.Build()
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IHomeService, HomeService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

// ✅ Now build the app
var app = builder.Build();

// Seed admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Admins.Any())
    {
        var hasher = new PasswordHasher<Admin>();
        var admin = new Admin { Email = "admin2025@redkozhi" };
        admin.PasswordHash = hasher.HashPassword(admin, "STR@2025");
        db.Admins.Add(admin);
        db.SaveChanges();
    }
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Important: after routing, before endpoints

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Needed for Identity login, register pages

app.Run();
