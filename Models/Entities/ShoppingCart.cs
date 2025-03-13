using System;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class ShoppingCart
    {

        // You can add extra properties for the user here
        public int ShoppingCartID { get; set; }
        // public string Name { get; set; }
        // public string User_id { get; set; }
        public string UserID { get; set; }
                public ApplicationUser User { get; set; }


        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        // public string LastName { get; set; }
        // public string Email { get; set; }
        // public string Password { get; set; }
                public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        

    }
}