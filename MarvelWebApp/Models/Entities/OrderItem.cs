using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class OrderItem
    {

        // You can add extra properties for the user here
        public int OrderItemID {get; set;}
        public int OrderID { get; set; }
        public Order Order { get; set; }

        // public string User_id { get; set; }
        // public DateTime Order_date {get; set;}
        // public int Total_price {get; set;}
        // public int ComicID {get; set;}
        public int ComicID {get; set;}
                public Comic Comic { get; set; }

        public int Quantity {get; set;}
        //p 

public decimal PriceAtPurchase {get; set;}


        // public string LastName { get; set; }
        // public string Email { get; set; }
        // public string Password { get; set; }
        

    }
}