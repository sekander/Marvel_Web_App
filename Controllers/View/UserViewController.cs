using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class UserViewController : Controller
    {
        public IActionResult Dashboard()
        {
            Console.WriteLine("User Dashboard");
            return View();
        }
    }
}
