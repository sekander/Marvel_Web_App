using Microsoft.AspNetCore.Mvc;
using MarvelWebApp.Models;
using MarvelWebApp.Interface;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MarvelWebApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserAPIController : ControllerBase 
    {
        private readonly IEntityService<Comic> _comicService;
        private readonly IEntityService<ShoppingCart> _cartService;
        private readonly IEntityService<ShoppingCartItem> _cartItemService;
        private readonly IEntityService<Order> _orderService;
        private readonly IEntityService<Payment> _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        private ShoppingCart userCart;
        private List<ShoppingCartItem> userCartItems;

        public UserAPIController(
            IEntityService<Comic> comicService,
            IEntityService<ShoppingCart> cartService,
            IEntityService<ShoppingCartItem> cartItemService,
            IEntityService<Order> orderService,
            IEntityService<Payment> paymentService,
            UserManager<ApplicationUser> userManager)
        {
            _comicService = comicService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _orderService = orderService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        // 1. View all comics
        // [HttpGet("comics")]
        // public async Task<IActionResult> ViewComics()
        // {
        //     var comics = await _comicService.GetAllEntityAsync();
        //     return Ok(comics); // Return a list of comics as JSON
        // }

        [HttpGet("comics")]
        public async Task<IActionResult> ViewComics([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var comics = await _comicService.GetAllEntityAsync();
            var pagedComics = comics
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = comics.Count;

            return Ok(new
            {
                comics = pagedComics,
                totalCount,
                currentPage = page,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

           // 2. Search for comics by title, genre, or other properties
        [HttpGet("comics/search")]
        public async Task<IActionResult> SearchComics([FromQuery] string title)
        {
            var comics = await _comicService.GetAllEntityAsync();

            // Filter by title or genre if provided
            if (!string.IsNullOrEmpty(title))
            {
                comics = comics.Where(c => c.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!comics.Any())
            {
                return NotFound(new { message = "No comics found matching your criteria." });
            }

            return Ok(comics); // Return filtered comics as JSON
        }

        // GET: api/manager/comic/id
        [HttpGet("comic/{id}")]
        public async Task<IActionResult> GetComic(int id)
        {
            var comic = await _comicService.GetEntityByIdAsync(id);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }

            return Ok(new {comic});
        }



        // 2. Add comic to the shopping cart
        // [HttpPost("shopping-cart/add/{comicId}")]
        [HttpPost("shopping-cart/add")]
        public async Task<IActionResult> AddToCart(int comicId, int quantity)
        {
            
            var user = HttpContext.User;
            if (user?.Identity?.Name == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var appUser = await _userManager.FindByEmailAsync(user.Identity.Name);
            if (appUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var allCarts = await _cartService.GetAllEntityAsync();
            userCart = allCarts.FirstOrDefault(c => c.UserID == appUser.Id);

            if (userCart == null)
            {
                userCart = new ShoppingCart
                {
                    UserID = appUser.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _cartService.CreateEntityAsync(userCart);
            }
            else
            {
                userCart.UpdatedAt = DateTime.Now;
                await _cartService.UpdateEntityAsync(userCart);
            }
            Console.WriteLine(comicId);

            // var comic = await _comicService.GetEntityByIdAsync(comicId);
            var comic = await _comicService.GetEntityByIdAsync(3);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }

            var cartItem = new ShoppingCartItem
            {
                ShoppingCartID = userCart.ShoppingCartID,
                ComicID = comic.ComicID,
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PriceAtAdd = comic.Price
            };
            await _cartItemService.CreateEntityAsync(cartItem);

            var allItems = await _cartItemService.GetAllEntityAsync();
            userCartItems = allItems.Where(i => i.ShoppingCartID == userCart.ShoppingCartID).ToList();

            var allComics = await _comicService.GetAllEntityAsync();

            // ✅ Debug log: print all items in this user's cart
            Console.WriteLine($"=== Cart Items for User ID: {appUser.Id} ===");
            foreach (var item in userCartItems)
            {
                var relatedComic = allComics.FirstOrDefault(c => c.ComicID == item.ComicID);
                Console.WriteLine($"ItemID: {item.ShoppingCartItemID}, Comic: {relatedComic?.Title}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}, Added: {item.CreatedAt}");
            }
            Console.WriteLine("=== End of Cart Items ===");

            return Ok(new { message = "Comic added to shopping cart successfully" });


            /*
            // var user = await _userManager.GetUserAsync(User);
            var user = HttpContext.User;

            // var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            
            
            
            var userId = await _userManager.FindByEmailAsync(user.Identity.Name);
            Console.WriteLine($"User ID: {userId}");
            // var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            Console.WriteLine($"User ID: {userId}");
            // Console.WriteLine($"User Name: {user.UserName}");

            if (user == null)
            {
                Console.WriteLine("User not found");
                return Unauthorized(); // Ensure user is logged in
            }
                
            userCart = new ShoppingCart
            {
                // UserID = user.Id,
                UserID = userId?.Id ?? throw new InvalidOperationException("User ID cannot be null"),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            // await _cartService.CreateEntityAsync(cart);
            Console.WriteLine("User ID: " + userId?.Id + 
                              " Cart ID: " + userCart.ShoppingCartID + 
                              " CreatedAt: " + userCart.CreatedAt + 
                              " UpdatedAt: " + userCart.UpdatedAt);
            await _cartService.CreateEntityAsync(userCart);
            // }
            Console.WriteLine("Comic ID: " + comicId);
            // Fetch comic from the database
            // var comic = await _comicService.GetEntityByIdAsync(comicId);
            var comic = await _comicService.GetEntityByIdAsync(3);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" }); // Comic not found
            }
            // Add the comic to the shopping cart
            var cartItem = new ShoppingCartItem
            {
                ShoppingCartID = userCart.ShoppingCartID,
                ComicID = comic.ComicID,
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PriceAtAdd = comic.Price
            };

            await _cartItemService.CreateEntityAsync(cartItem);

            // Fetch all items in the cart
            var cartItems = await _cartItemService.GetAllEntityAsync();
            // var userCartItems = cartItems.Where(item => item.ShoppingCartID == userCart.ShoppingCartID).ToList();

            userCartItems = [cartItem];

            userCartItems = cartItems.Where(item => item.ShoppingCartID == userCart.ShoppingCartID).ToList();

            Console.WriteLine("Items in cart:");
            foreach (var item in userCartItems)
            {
                Console.WriteLine($"Item ID: {item.ShoppingCartItemID}, Comic ID: {item.ComicID}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd}");
            }

            return Ok(new { message = "Comic added to shopping cart successfully" });
            */
        }

        // 3. Checkout and make a purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }

            // var carts = _cartService.GetAllEntityAsync();
            // var cart;
            // // Clear the shopping cart after successful purchase
            // foreach (var item in carts)
            // {
            //     // await _cartItemService.DeleteEntityAsync(item.ShoppingCartItemID);
            //     if (item.UserID == user.Id){
            //         cart = item;
            //     }
                    

            // }
            

            // Fetch the shopping cart and its items
            // var cart = await _cartService.GetEntityByIdAsync(user.Id);
            // if (cart == null || !cart.ShoppingCartItems.Any())
            // {
                // return BadRequest(new { message = "Shopping cart is empty" });
            // }

            // Calculate the total price of the cart
            // decimal totalPrice = cart.ShoppingCartItems.Sum(item => item.PriceAtAdd * item.Quantity);
            decimal totalPrice = userCart.ShoppingCartItems.Sum(item => item.PriceAtAdd * item.Quantity);

            // Create an order
            var order = new Order
            {
                UserID = user.Id,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                // OrderItems = cart.ShoppingCartItems.Select(item => new OrderItem
                OrderItems = userCart.ShoppingCartItems.Select(item => new OrderItem
                {
                    ComicID = item.ComicID,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.PriceAtAdd
                }).ToList()
            };

            await _orderService.CreateEntityAsync(order);

            // Create a payment
            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentDate = DateTime.Now,
                PaymentAmount = totalPrice,
                TransactionID = new Random().Next(100000, 999999) // Example transaction ID
            };

            await _paymentService.CreateEntityAsync(payment);

            // Clear the shopping cart after successful purchase
            // foreach (var item in cart.ShoppingCartItems)
            foreach (var item in userCart.ShoppingCartItems)
            {
                await _cartItemService.DeleteEntityAsync(item.ShoppingCartItemID);
            }

            return Ok(new { message = "Purchase successful" });
        }


        // Get current user's cart
        [HttpGet("shopping-cart")]
        public async Task<IActionResult> GetCart()
        {
            var user = HttpContext.User;
            if (user?.Identity?.Name == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var appUser = await _userManager.FindByEmailAsync(user.Identity.Name);
            if (appUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var allCarts = await _cartService.GetAllEntityAsync();
            var userCart = allCarts.FirstOrDefault(c => c.UserID == appUser.Id);

            if (userCart == null)
            {
                Console.WriteLine("User has no cart.");
                return Ok(new List<ShoppingCartItem>());
            }

            var allItems = await _cartItemService.GetAllEntityAsync();
            // var userCartItems = allItems.Where(i => i.ShoppingCartID == userCart.ShoppingCartID).ToList();

            // Debug print cart details
            Console.WriteLine("=== GetCart ===");
            Console.WriteLine("User ID: " + userCart.UserID);
            Console.WriteLine("Cart ID: " + userCart.ShoppingCartID);
            Console.WriteLine("CreatedAt: " + userCart.CreatedAt);
            Console.WriteLine("UpdatedAt: " + userCart.UpdatedAt);
            Console.WriteLine("Items in cart:");
            userCartItems = allItems.Where(i => i.ShoppingCartID == userCart.ShoppingCartID).ToList();

            var allComics = await _comicService.GetAllEntityAsync();

            // ✅ Debug log: print all items in this user's cart
            Console.WriteLine($"=== Cart Items for User ID: {appUser.Id} ===");
            foreach (var item in userCartItems)
            {
                var relatedComic = allComics.FirstOrDefault(c => c.ComicID == item.ComicID);
                Console.WriteLine($"ItemID: {item.ShoppingCartItemID}, Comic: {relatedComic?.Title}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}, Added: {item.CreatedAt}");
            }
            Console.WriteLine("=== End of Cart Items ===");

            var itemDtos = userCartItems.Select(item => new
            {
                item.ShoppingCartItemID,
                item.ComicID,
                ComicTitle = allComics.FirstOrDefault(c => c.ComicID == item.ComicID)?.Title,
                item.Quantity,
                item.PriceAtAdd,
                item.CreatedAt
            }).ToList();


            // ✅ Debug log everything clearly
            Console.WriteLine($"=== Cart Items for User ID: {appUser.Id} ===");
            foreach (var dto in itemDtos)
            {
                Console.WriteLine($"ItemID: {dto.ShoppingCartItemID}, ComicID: {dto.ComicID}, Title: {dto.ComicTitle}, Quantity: {dto.Quantity}, Price: {dto.PriceAtAdd:C}, Added: {dto.CreatedAt}");
            }
            Console.WriteLine("=== End of Cart Items ===");

            return Ok(itemDtos);

            // foreach (var item in userCartItems)
            // {
            //     Console.WriteLine($"Item ID: {item.ShoppingCartItemID}, Comic ID: {item.ComicID}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}, Added: {item.CreatedAt}");
            // }
            // Console.WriteLine("=== End of Cart ===");

            // return Ok(userCartItems);
        }

        // Remove item from cart
        [HttpDelete("shopping-cart/remove/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var item = await _cartItemService.GetEntityByIdAsync(itemId);
            if (item == null) return NotFound();

            await _cartItemService.DeleteEntityAsync(itemId);
            return Ok(new { message = "Item removed from cart." });
        }
    }
}
