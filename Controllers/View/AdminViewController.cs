using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminViewController : Controller
    {
        // Admin Dashboard
            [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            Console.WriteLine("Admin Dashboard");
            // You can add business logic here, such as fetching data or statistics.
            return View("../Admin/Dashboard");
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