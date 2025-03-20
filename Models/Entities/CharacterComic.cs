using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarvelWebApp.Models
{
      // Join Table for Many-to-Many Relationship
    public class CharacterComic
    {
        [Key]
        public int CharacterID { get; set; }
        public Character Character { get; set; }

        public int ComicID { get; set; }
        public Comic Comic { get; set; }
    }
}