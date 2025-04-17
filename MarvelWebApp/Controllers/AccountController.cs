using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarvelWebApp.Models;
using System.Threading.Tasks;

namespace MarvelWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        [Route("Account/Login")]
        public IActionResult Login()
        {
            Console.WriteLine("Login attempt TESTING ");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
            [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine("Login attempt");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                        Console.WriteLine("User exists");
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        Console.WriteLine("Login Success");

                        // return RedirectToAction("Dashboard", "Account");
                        // Check user roles and redirect accordingly
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            Console.WriteLine("Admin Success");
                            // return View("Admin/Dashboard");
                                    return RedirectToAction("Dashboard", "Admin");
                                    // return RedirectToAction("Dashboard", "AdminView");
                                    // return RedirectToAction("Dashboard", "AdminView/Dashboard");
                                    // return RedirectToAction("Dashboard", "Admin/Dashboard");
                            

                        }
                        else if (roles.Contains("Manager"))
                        {
                            Console.WriteLine("Manager Success");
                            return RedirectToAction("Dashboard", "Manager");
                        }
                        else if (roles.Contains("User"))
                        {
                            Console.WriteLine("User Success");
                            return RedirectToAction("Dashboard", "User");
                        }
                        else
                        {
                            Console.WriteLine("Default Success");
                            // Default dashboard for unclassified roles
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                            Console.WriteLine("Default Success");
                        ViewData["ErrorMessage"] = "Invalid login attempt.";
                    }
                }
                else
                {
                            Console.WriteLine("Default Success");
                    ViewData["ErrorMessage"] = "User does not exist.";
                }
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}