
using Microsoft.AspNetCore.Identity;



namespace MarvelWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {

        // You can add extra properties for the user here
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // public string Email { get; set; }
        // public string Password { get; set; }
        
         // User-specific properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Comic> ComicsCollection { get; set; }

    }
}

