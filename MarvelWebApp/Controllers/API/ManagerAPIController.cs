using Microsoft.AspNetCore.Mvc;
using MarvelWebApp.Models;
using MarvelWebApp.Interface;
using System.Threading.Tasks;

namespace MarvelWebApp.Controllers
{
    [Route("api/manager")]
    [ApiController]
    // [Authorize(Roles = "Manager")]
    public class ManagerAPIController : BaseEntityController<Comic>
    {
        private readonly IEntityService<Comic> _entityService;

        // Pass it to the base controller
        public ManagerAPIController(IEntityService<Comic> entityService)
            : base(entityService) 
        {
            _entityService = entityService;
        }

        // GET: api/manager/comic/id
        [HttpGet("comic/{id}")]
        public async Task<IActionResult> GetComic(int id)
        {
            var comic = await _entityService.GetEntityByIdAsync(id);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }

            return Ok(new {comic});
        }

        // POST: api/manager/comics
        [HttpPost("comics")]
        public async Task<IActionResult> AddComic([FromBody] Comic comic)
        {
            if (comic == null)
            {
                return BadRequest(new { message = "Invalid comic data" });
            }

        // Create the comic using only the relevant properties
            var newComic = new Comic
            {
                Title = comic.Title,
                Quantity = comic.Quantity,
                Price = comic.Price,
                 // Handle null values for optional fields
                Artist = string.IsNullOrEmpty(comic.Artist) ? null : comic.Artist,
                Series = string.IsNullOrEmpty(comic.Series) ? null : comic.Series,
                Writer = string.IsNullOrEmpty(comic.Writer) ? null : comic.Writer,
                Description = string.IsNullOrEmpty(comic.Description) ? null : comic.Description,
                ThumbnailURL = string.IsNullOrEmpty(comic.ThumbnailURL) ? null : comic.ThumbnailURL,
                CoverImageUrl = string.IsNullOrEmpty(comic.CoverImageUrl) ? null : comic.CoverImageUrl,
                DetailsURL = string.IsNullOrEmpty(comic.DetailsURL) ? null : comic.DetailsURL,


                // You can add other relevant properties as needed
            };
            Console.WriteLine("Comic added: " + newComic.Title);

            // Assuming _entityService.CreateEntityAsync is properly set up to save the comic
            await _entityService.CreateEntityAsync(newComic);


            return Ok(new { message = "Comic added successfully" });
        }

        // PUT: api/manager/comic/{id}
        [HttpPut("comic/{id}")]
        public async Task<IActionResult> UpdateComicQuantity(int id, [FromBody] int quantity)
        {
            var comic = await _entityService.GetEntityByIdAsync(id);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }

            comic.Quantity = quantity;
            await _entityService.UpdateEntityAsync(comic);
            
            return Ok(new { message = "Comic quantity updated successfully" });
        }

        // DELETE: api/manager/comics/{id}
        [HttpDelete("comics/{id}")]
        public async Task<IActionResult> DeleteComic(int id)
        {
            var comic = await _entityService.GetEntityByIdAsync(id);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }
            
            await _entityService.DeleteEntityAsync(id);


            return Ok(new { message = "Comic deleted successfully" });
        }
    }
}
