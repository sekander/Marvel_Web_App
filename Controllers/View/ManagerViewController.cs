using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarvelWebApp.Models;
using MarvelWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MarvelWebApp.Controllers.View
{
    [Authorize(Roles = "Manager")]
  public class ManagerController : Controller
  {
    private readonly ILogger<ManagerController> _logger;
    private readonly ApplicationDbContext _context;

    public ManagerController(ILogger<ManagerController> logger, ApplicationDbContext context)
    {
      _logger = logger;
      _context = context;
    }

    /// <summary>
    /// Displays the manager dashboard with a list of all comics
    /// </summary>
    /// <returns>
    /// View containing a table of comics with their details
    /// </returns>
    /// <example>
    /// GET: Manager/Dashboard -> Returns view with comics table
    /// </example>
    /// <remarks>
    /// Route: /Manager/Dashboard
    /// Note: Only accessible to users with Manager role
    /// </remarks>
    public async Task<IActionResult> Dashboard()
    {
      _logger.LogInformation("Manager Dashboard accessed");
      var comics = await _context.Comics
          .Include(c => c.CharacterComics)
          .ThenInclude(cc => cc.Character)
          .ToListAsync();
      return View(comics);
    }

    /// <summary>
    /// Displays the form to create a new comic
    /// </summary>
    /// <returns>
    /// View containing the comic creation form
    /// </returns>
    /// <example>
    /// GET: Manager/Create -> Returns comic creation form
    /// </example>
    /// <remarks>
    /// Route: /Manager/Create
    /// Note: Form includes fields for all comic properties
    /// </remarks>
    public IActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Processes the comic creation form submission
    /// </summary>
    /// <param name="comic">The comic object created from form data</param>
    /// <returns>
    /// Redirects to Dashboard on success
    /// Returns to form with errors if validation fails
    /// </returns>
    /// <example>
    /// POST: Manager/Create -> Redirects to Dashboard if successful
    /// </example>
    /// <remarks>
    /// Route: /Manager/Create
    /// Note: Includes anti-forgery token validation
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Comic comic)
    {
      if (ModelState.IsValid)
      {
        try
        {
          _context.Comics.Add(comic);
          await _context.SaveChangesAsync();
          _logger.LogInformation($"New comic added: {comic.Title}");
          return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error adding new comic");
          ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
        }
      }
      return View(comic);
    }

    /// <summary>
    /// Displays the form to edit an existing comic
    /// </summary>
    /// <param name="id">The ID of the comic to edit</param>
    /// <returns>
    /// View containing the comic edit form
    /// Returns NotFound if comic doesn't exist
    /// </returns>
    /// <example>
    /// GET: Manager/Edit/5 -> Returns edit form for comic with ID 5
    /// </example>
    /// <remarks>
    /// Route: /Manager/Edit/{id}
    /// Note: Form pre-populated with existing comic data
    /// </remarks>
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var comic = await _context.Comics.FindAsync(id);
      if (comic == null)
      {
        return NotFound();
      }
      return View(comic);
    }

    /// <summary>
    /// Processes the comic edit form submission
    /// </summary>
    /// <param name="id">The ID of the comic being edited</param>
    /// <param name="comic">The updated comic object</param>
    /// <returns>
    /// Redirects to Dashboard on success
    /// Returns to form with errors if validation fails
    /// </returns>
    /// <example>
    /// POST: Manager/Edit/5 -> Redirects to Dashboard if successful
    /// </example>
    /// <remarks>
    /// Route: /Manager/Edit/{id}
    /// Note: Includes anti-forgery token validation
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Comic comic)
    {
      if (id != comic.ID)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(comic);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error updating comic");
          ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
        }
      }
      return View(comic);
    }

    /// <summary>
    /// Displays the delete confirmation page for a comic
    /// </summary>
    /// <param name="id">The ID of the comic to delete</param>
    /// <returns>
    /// View containing delete confirmation
    /// Returns NotFound if comic doesn't exist
    /// </returns>
    /// <example>
    /// GET: Manager/Delete/5 -> Returns delete confirmation for comic with ID 5
    /// </example>
    /// <remarks>
    /// Route: /Manager/Delete/{id}
    /// Note: Shows comic details before deletion
    /// </remarks>
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var comic = await _context.Comics
          .FirstOrDefaultAsync(m => m.ID == id);
      if (comic == null)
      {
        return NotFound();
      }

      return View(comic);
    }

    /// <summary>
    /// Processes the comic deletion
    /// </summary>
    /// <param name="id">The ID of the comic to delete</param>
    /// <returns>
    /// Redirects to Dashboard on success
    /// Returns NotFound if comic doesn't exist
    /// </returns>
    /// <example>
    /// POST: Manager/Delete/5 -> Redirects to Dashboard if successful
    /// </example>
    /// <remarks>
    /// Route: /Manager/Delete/{id}
    /// Note: Includes anti-forgery token validation
    /// </remarks>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var comic = await _context.Comics.FindAsync(id);
      if (comic != null)
      {
        _context.Comics.Remove(comic);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Dashboard));
    }
  }
}