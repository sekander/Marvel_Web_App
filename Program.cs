using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarvelWebApp.Data;
using MarvelWebApp.Models;
using MarvelWebApp.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure MySQL with Pomelo
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25)))); // Replace with your MySQL version

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));


var app = builder.Build();

// Seed the database with roles and default users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Initialize the database with roles and a default admin user
    await DbInitializer.Initialize(services, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.UseAuthentication();
app.UseAuthorization();

// Redirect unauthenticated users to login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// app.MapControllerRoute(
//     name: "login",
//     pattern: "Account/Login",
//     defaults: new { controller = "Account", action = "Login" });

// app.MapControllerRoute(
//     name: "logout",
//     pattern: "Admin/Logout",
//     defaults: new { controller = "Admin", action = "Logout" });

app.MapFallbackToController("Login", "Account");

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

