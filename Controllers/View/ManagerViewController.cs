using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarvelWebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerViewController : Controller
    {
        public IActionResult Dashboard()
        {
            Console.WriteLine("Manager Dashboard");
            return View();
        }
    }
}
