// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using MarvelWebApp.Models;
// using MarvelWebApp.Data;
// using Microsoft.EntityFrameworkCore;

// namespace MarvelWebApp.Controllers.View
// {
//   [Authorize(Roles = "Manager")]
//   public class ManagerController : Controller
//   {
//     private readonly ILogger<ManagerController> _logger;
//     private readonly ApplicationDbContext _context;

//     public ManagerController(ILogger<ManagerController> logger, ApplicationDbContext context)
//     {
//       _logger = logger;
//       _context = context;
//     }

//     // GET: Manager/Dashboard (MVC convention)
//     public async Task<IActionResult> Dashboard()
//     {
//       _logger.LogInformation("Manager Dashboard accessed");
//       var comics = await _context.Comics
//           .Include(c => c.CharacterComics)
//           .ThenInclude(cc => cc.Character)
//           .ToListAsync();
//       return View(comics);
//     }

//     // GET: Manager/Create
//     public IActionResult Create()
//     {
//       return View();
//     }

//     // POST: Manager/Create
//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> Create(Comic comic)
//     {
//        // Debugging model validation issues
//       if (!ModelState.IsValid)
//       {
//           foreach (var modelState in ModelState)
//           {
//               foreach (var error in modelState.Value.Errors)
//               {
//                   Console.WriteLine($"Property: {modelState.Key}, Error: {error.ErrorMessage}");
//                   _logger.LogWarning($"Validation error on {modelState.Key}: {error.ErrorMessage}");
//               }
//           }
//       }

//       if (ModelState.IsValid)
//             {
//                 var comicObj = new Comic
//                 {
//                   Title = comic.Title,
//                   Series = comic.Series,
//                   IssueNumber = comic.IssueNumber,
//                   Description = comic.Description,
//                   Price = comic.Price,
//                   Quantity = comic.Quantity,
//                   ReleaseDate = comic.ReleaseDate,
//                   Writer = comic.Writer,
//                   Artist = comic.Artist
//                 };

//                 _context.Comics.Add(comicObj);
//                 await _context.SaveChangesAsync();

//                 return RedirectToAction("Dashboard", "Manager");  // Redirect to the list of landlords
//             }
//             return View(comic);
      


//     }

//     // [HttpPost]

//   }
// }
