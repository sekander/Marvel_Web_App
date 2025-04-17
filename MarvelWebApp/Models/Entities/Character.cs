using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class Character
    {
        public int CharacterID { get; set; }
        public string name {get; set;}
        // public List<string> ComicIDS {get; set;}

        // Navigation property for many-to-many relationship
        public ICollection<CharacterComic> CharacterComics { get; set; } = new List<CharacterComic>();

        
    }
}