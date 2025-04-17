using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarvelWebApp.Models;
using MarvelWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MarvelWebApp.Controllers.View
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminViewController : Controller
    {
        private readonly ILogger<AdminViewController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminViewController(
            ILogger<AdminViewController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays the admin dashboard with a list of all users
        /// </summary>
        /// <returns>
        /// View containing a table of users with their details
        /// </returns>
        /// <example>
        /// GET: Admin/Dashboard -> Returns view with users table
        /// </example>
        /// <remarks>
        /// Route: /Admin/Dashboard
        /// Note: Only accessible to users with Admin role
        /// </remarks>
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            _logger.LogInformation("Admin Dashboard accessed");
            var users = await _userManager.Users
                .Include(u => u.Orders)
                .Include(u => u.ComicsCollection)
                .ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Displays the form to edit an existing user
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        /// <returns>
        /// View containing the user edit form
        /// Returns NotFound if user doesn't exist
        /// </returns>
        /// <example>
        /// GET: Admin/Edit/5 -> Returns edit form for user with ID 5
        /// </example>
        /// <remarks>
        /// Route: /Admin/Edit/{id}
        /// Note: Form pre-populated with existing user data
        /// </remarks>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Processes the user edit form submission
        /// </summary>
        /// <param name="id">The ID of the user being edited</param>
        /// <param name="user">The updated user object</param>
        /// <returns>
        /// Redirects to Dashboard on success
        /// Returns to form with errors if validation fails
        /// </returns>
        /// <example>
        /// POST: Admin/Edit/5 -> Redirects to Dashboard if successful
        /// </example>
        /// <remarks>
        /// Route: /Admin/Edit/{id}
        /// Note: Includes anti-forgery token validation
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userManager.FindByIdAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.UserName = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(user);
                    }

                    return RedirectToAction(nameof(Dashboard));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user");
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }
            return View(user);
        }

        /// <summary>
        /// Displays the delete confirmation page for a user
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>
        /// View containing delete confirmation
        /// Returns NotFound if user doesn't exist
        /// </returns>
        /// <example>
        /// GET: Admin/Delete/5 -> Returns delete confirmation for user with ID 5
        /// </example>
        /// <remarks>
        /// Route: /Admin/Delete/{id}
        /// Note: Shows user details before deletion
        /// </remarks>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Processes the user deletion
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>
        /// Redirects to Dashboard on success
        /// Returns NotFound if user doesn't exist
        /// </returns>
        /// <example>
        /// POST: Admin/Delete/5 -> Redirects to Dashboard if successful
        /// </example>
        /// <remarks>
        /// Route: /Admin/Delete/{id}
        /// Note: Includes anti-forgery token validation
        /// </remarks>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
            }
            return RedirectToAction(nameof(Dashboard));
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