using MarvelWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminViewController : Controller
    {

        private readonly HttpClient _httpClient;


        //    private readonly HttpClient _httpClient;

        // Inject HttpClient through dependency injection
        public AdminViewController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Admin Dashboard
        [Route("Dashboard")]
        public async Task<IActionResult> DashboardAsync()
        {
            Console.WriteLine("Admin Dashboard");

            var response = await _httpClient.GetStringAsync("http://localhost:5031/api/admin/users/");
             // Make an HTTP request to a remote API or another controller



            // You can add business logic here, such as fetching data or statistics.
            return View("../Admin/Dashboard");
        }

        // Admin-specific settings or actions could go here
        [Route("GetUser")]
        public async Task<IActionResult> GetUser(string email)
        {
            var response = await _httpClient.GetStringAsync("http://localhost:5031/api/admin/users/email/" + email);

            // Deserialize JSON response
            // var myData = JsonSerializer.Deserialize<ApplicationUser>(response);
            // var myData = JsonSerializer.Deserialize<ApplicationUser>(response);

            Console.WriteLine(response);

               // Parse JSON and get value by key
            var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
        
            // Example: Get "userId" value
            string userId = root.GetProperty("userId").GetString();
        
            Console.WriteLine(userId);


            var get_user = await _httpClient.GetStringAsync("http://localhost:5031/api/admin/users/" + userId);

            Console.WriteLine(get_user);
            doc = JsonDocument.Parse(get_user);
            root = doc.RootElement;
            var user = root.GetProperty("user");    
        
            // Example: Get "userId" value
            string firstName = user.GetProperty("firstName").GetString();
            string lastName = user.GetProperty("lastName").GetString();
            string userName = user.GetProperty("userName").GetString();

            // Pass the data to the view using ViewData
            ViewData["MyData"] = response;

             // Create an anonymous object with the extracted data
            var userData = new
            {
                firstName,
                lastName,
                userName
            };
        
            // Pass the JSON-like object to the view using ViewData
            ViewData["UserData"] = JsonSerializer.Serialize(userData);

            // return View();
            return View("../Admin/Dashboard");
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