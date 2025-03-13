using Microsoft.EntityFrameworkCore;
using MarvelWebApp.Models;

namespace MarvelWebApp.Interface {

    //https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-interfaces


    // Interface for defining service methods that can work with any entity type `T` (where T is a class).
    public interface IEntityService<T> where T : class 
    {
        // Asynchronously retrieve a list of all entities of type T from the database.
        Task<List<T>> GetAllEntityAsync();
        // Asynchronously retrieve a single entity of type T by its ID.
        Task<T> GetEntityByIdAsync(int id);
        // Asynchronously create a new entity of type T in the database.
        Task CreateEntityAsync(T entity);
        // Asynchronously update an existing entity of type T in the database.
        Task UpdateEntityAsync(T entity);
        // Asynchronously delete an entity of type T from the database by its ID.
        Task DeleteEntityAsync(int id);
        // Asynchronously save all changes made to the entities in the context.
        Task SaveChangesAsync();

    }
}