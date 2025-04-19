
using System;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class ShoppingCartItem
    {

        // You can add extra properties for the user here
        public int ShoppingCartItemID { get; set; }
        public int ShoppingCartID { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        public int ComicID { get; set; }
        public Comic Comic { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        public decimal PriceAtAdd { get; set; }
    }
}