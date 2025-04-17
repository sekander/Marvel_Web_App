using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MarvelWebApp.Interface;
using MarvelWebApp.Models;


/*
SUMMARY
    This BaseEntityController<T> class is a generic API controller used to handle 
    CRUD operations for different entities (T). It provides common actions like 
    retrieving all entities, retrieving an entity by ID, creating, updating, 
    and deleting entities. It also includes a validation method and a mechanism to 
    save changes to the database.
*/

[Route("api/[controller]")]
[ApiController]
// public class BaseEntityController<T> : ControllerBase where T : class
public class BaseEntityController<T> : Controller where T : class
{
    private readonly IEntityService<T> _entityService;

    // Provides access to the entity service used for database operations
    public IEntityService<T> GetService(){return _entityService;}

    // Enum to handle validation states for Create and Update operations
    protected enum ValidationState
    {
        CREATE,
        UPDATE
    }
    
    // Default message to be returned during validation errors
    protected string return_message = "";
    // Flag to indicate if the controller is in its initial state (used for optimization)
    protected static bool _controllerInitialState = false;
    
    // Protected constructor to ensure derived classes can initialize the service
    protected BaseEntityController(IEntityService<T> entityService)
    {
        _entityService = entityService;
    }



    /// <summary>
    /// Retrieves all entities of type T from the database.
    /// </summary>
    /// <returns>Returns a list of all entities of type T.</returns>
    [HttpGet]
    public async virtual Task<IActionResult> GetAllEntities()
    {
        var entities = await _entityService.GetAllEntityAsync();
        return Ok(entities);
        
    }

    /// <summary>
    /// Retrieves a specific entity of type T by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>Returns the entity with the specified ID, or an error message if not found.</returns>
    [HttpGet("{id}")]
    public async virtual Task<IActionResult> GetEntity(int id)
    {
        var entity = await _entityService.GetEntityByIdAsync(id);
        if ( id <= 0)
        {
            return BadRequest(new  {
                success = false,
                data = id,
                message = "Please Enter an ID greater than 0",
                requestedId = id
            });
        }
        else if (entity == null) return BadRequest(new {
                success = false,
                data = id,
                message = "ID not found",
                requestedId = id
            });
        else{
            return Ok(entity);
        }
        // return (IActionResult)entity;
    }

    /// <summary>
    /// Creates a new entity of type T in the database.
    /// </summary>
    /// <param name="entity">The entity object to create.</param>
    /// <returns>Returns a success message with the created entity.</returns>
    [HttpPost]
    public async virtual Task<IActionResult> CreateEntity([FromBody] T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Invalid data",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }
        else{
            await _entityService.CreateEntityAsync(entity);
            // return CreatedAtAction(nameof(GetEntity), new { id = entity.GetType().GetProperty("ID").GetValue(entity) }, entity);
            return Ok(new 
            {
                success = true,
                data = entity,
                message = "Created: " + entity,
            });
        }
    }

    /// <summary>
    /// Updates an existing entity of type T in the database.
    /// </summary>
    /// <param name="entity">The entity object to update.</param>
    /// <returns>Returns a success message with the updated entity.</returns>
    [HttpPut]
    public async virtual Task<IActionResult> UpdateEntity([FromBody] T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Invalid data",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }
        else {
            // await _entityService.UpdateEntityAsync(id, entity);
            await _entityService.UpdateEntityAsync(entity);
            return Ok(new 
            {
                success = true,
                data = entity,
                message = "Updated: " + entity,
            });
        }
    }

    /// <summary>
    /// Deletes a specific entity of type T by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>Returns a success message if the entity was deleted, or an error message if not found.</returns>
    [HttpDelete("{id}")]
    public async virtual Task<IActionResult> DeleteEntity(int id)
    {
        var entity = await _entityService.GetEntityByIdAsync(id);

        if ( id <= 0)
        {
            return BadRequest(new  
            {
                success = false,
                data = id,
                message = "Please Enter an ID greater than 0",
                requestedId = id
            });
        }
        else if (entity == null) return BadRequest(new {
                success = false,
                data = id,
                message = "ID not found",
                requestedId = id
            });
        else {
            await _entityService.DeleteEntityAsync(id);
            return Ok(new 
            {
                success = true,
                data = id,
                message = "Deleted : " + id,
                requestedId = id
            });
        }
    }
}