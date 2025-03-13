using Microsoft.EntityFrameworkCore;
using MarvelWebApp.Data;
using MarvelWebApp.Interface;
using MarvelWebApp.Models;


/*
SUMMARY
    This class, EntityService<T>, is an implementation of the IEntityService<T> interface, 
    designed to manage entities of type T within the database using Entity Framework Core. 
    This service class allows CRUD (Create, Read, Update, Delete) operations on the database, 
    leveraging asynchronous methods for efficient execution.
*/
public class EntityService<T> : IEntityService<T> where T : class 
{
    private readonly ApplicationDbContext _db;
    // private 

    public EntityService(ApplicationDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));  // Ensure db is properly injected and is not null
        // var test = _db.Set<T>();
    }


    // // Retrieve DbSets for specific entity types
    // public DbSet<Player> GetPlayers(){return _db.Players;}
    // public DbSet<Deck> GetDecks(){return _db.Decks;}
    // public DbSet<Card> GetCards(){return _db.Cards;}
    // public DbSet<CardDeck> GetCardDecks(){return _db.CardDecks;}

    // Fetch all entities of type T asynchronously
    async Task<List<T>> IEntityService<T>.GetAllEntityAsync()
    {
        return await _db.Set<T>().ToListAsync();
    }

    // Fetch a single entity by its ID asynchronously
    async Task<T?> IEntityService<T>.GetEntityByIdAsync(int id)
    {
        return await _db.Set<T>().FindAsync(id);
    }

    // Add a new entity to the database asynchronously
    public async Task CreateEntityAsync(T entity)
    {
        _db.Set<T>().Add(entity);
        // await _db.SaveChangesAsync();
        await SaveChangesAsync();
    }

    // Update an existing entity asynchronously
    public async Task UpdateEntityAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        // await _db.SaveChangesAsync();
        await SaveChangesAsync();
    }

    // Delete an entity by ID asynchronously
    public async Task DeleteEntityAsync(int id)
    {
        var entity = await _db.Set<T>().FindAsync(id);
        _db.Set<T>().Remove(entity);
        // await _db.SaveChangesAsync();
        await SaveChangesAsync();
    }

    // Save changes asynchronously to the database
    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}