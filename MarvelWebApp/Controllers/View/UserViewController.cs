using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "User")]
    [Route("User")]
    public class UserViewController : Controller
    {
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            Console.WriteLine("User Dashboard");

            var user = HttpContext.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            Console.WriteLine($"User ID: {userId}");

            //User Debug
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                Console.WriteLine($"User Name: {user.Identity.Name}");
                
                Console.WriteLine($"Authentication Type: {user.Identity.AuthenticationType}");
                Console.WriteLine($"Is Authenticated: {user.Identity.IsAuthenticated}");

                Console.WriteLine("All Claims:");
                foreach (var claim in user.Claims)
                {
                    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated.");
            }


            // var token = await HttpContext.GetTokenAsync("access_token");
            // Console.WriteLine($"Authorization Token: {token}");
            return View("../Admin/UserDashboard");
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
