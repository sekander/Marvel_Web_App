using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class Order
    {

        // You can add extra properties for the user here
        public int OrderID { get; set; }
        public string OrderDetails { get; set; }

        public string UserID { get; set; }
        
        [JsonIgnore] // This prevents serialization loop
        public ApplicationUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }


        public DateTime OrderDate {get; set;}
        public decimal TotalPrice {get; set;}

        //p 




        // public string LastName { get; set; }
        // public string Email { get; set; }
        // public string Password { get; set; }
        

    }
}