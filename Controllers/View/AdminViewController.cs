using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MarvelWebApp.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using static MarvelWebApp.Controllers.AdminAPIController;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminViewController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private AdminAPIController adminAPIController;

        // Constructor with dependency injection
        public AdminViewController(UserManager<ApplicationUser> userManager, AdminAPIController _admin)
        {
            _userManager = userManager;
            adminAPIController = _admin;

        }
        
        // Admin Dashboard
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            Console.WriteLine("Admin Dashboard");
            // You can add business logic here, such as fetching data or statistics.
             // Fetch all users from the database
            // var users = await _userManager.Users.ToListAsync();
            var users = await adminAPIController.GetAllEntities();
            
            
            // Pass the users list to the view
            return View("../Admin/Dashboard", users);
            // return View("../Admin/Dashboard");
        }

        // GET: /Admin/CreateUser (show the form)
        [HttpGet]
        [Route("CreateUser")]
        public IActionResult CreateUser()
        {
            return View("../Admin/CreateUser");
        }

        [HttpPost]
        [Route("CreateUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserRequest model)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Admin-specific settings or actions could go here
            [Route("Settings")]
        public IActionResult Settings()
        {
            return View();
        }

        // Logout action
        [HttpPost]
        [ValidateAntiForgeryToken]
            [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Admin Logout");
            await HttpContext.SignOutAsync();

    // Set cache control headers to prevent caching of authenticated pages
    // Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    // Response.Headers["Pragma"] = "no-cache";
    // Response.Headers["Expires"] = "0";
    // Loop through all cookies in the request and delete them from the response
    foreach (var cookie in Request.Cookies)
    {
        Response.Cookies.Delete(cookie.Key);
        Console.WriteLine($"Deleted cookie: {cookie.Key}");
    }

    // Optionally, clear session data (if used)
    // HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // [HttpGet("/test")]
        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            Console.WriteLine("Admin ETST Logout");
            return Ok("Test");
        }
    }
}