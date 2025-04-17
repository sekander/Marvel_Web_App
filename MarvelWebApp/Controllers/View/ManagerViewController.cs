using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("Manager")]
    public class ManagerViewController : Controller
    {
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            Console.WriteLine("Manager Dashboard");
            // return View();
            // return View("../Account/User/Dashboard");
            return View("../Admin/ManagerDashboard");
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

    }
}
