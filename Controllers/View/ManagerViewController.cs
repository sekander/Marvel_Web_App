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

    public async Task<IActionResult> Dashboard()
    {
      _logger.LogInformation("Manager Dashboard accessed");
      var comics = await _context.Comics
          .Include(c => c.CharacterComics)
          .ThenInclude(cc => cc.Character)
          .ToListAsync();
      return View(comics);
    }

    // GET: Manager/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Manager/Create
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
  }
}
