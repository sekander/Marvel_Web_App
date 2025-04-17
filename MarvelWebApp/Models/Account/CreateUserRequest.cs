using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    public class CreateUserRequest
    {
        [NotMapped]
        // You can add extra properties for the user here
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        // public string Email { get; set; }
        // public string Password { get; set; }
        

    }
}

