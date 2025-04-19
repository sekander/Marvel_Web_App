
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class Comic
    {
        public int ID { get; set; }
        public int ComicID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int IssueNumber { get; set; }
        public string? Series { get; set; }
        public string? Description { get; set; }
        public int PageCount { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string? Writer { get; set; }
        public string? Artist { get; set; }
        public decimal Price { get; set; }
        public string? ThumbnailURL { get; set; }
        public string? DetailsURL { get; set; }
        public int Quantity { get; set; }
        public int CharacterID { get; set; }

        public string? CoverImageUrl { get; set; }

        // Optional navigation properties
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }

        // Many-to-Many Navigation Property
        public ICollection<CharacterComic> CharacterComics { get; set; } = new List<CharacterComic>();
    }
}
