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

            await _entityService.CreateEntityAsync(comic);
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
