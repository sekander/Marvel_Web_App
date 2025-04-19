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
        private readonly IEntityService<OrderItem> _orderItemService;
        private readonly IEntityService<Payment> _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        private ShoppingCart userCart;
        private List<ShoppingCartItem> userCartItems;

        public UserAPIController(
            IEntityService<Comic> comicService,
            IEntityService<ShoppingCart> cartService,
            IEntityService<ShoppingCartItem> cartItemService,
            IEntityService<Order> orderService,
            IEntityService<OrderItem> orderItemService,
            IEntityService<Payment> paymentService,
            UserManager<ApplicationUser> userManager)
        {
            _comicService = comicService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _orderService = orderService;
            _orderItemService = orderItemService;
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

            var comic = await _comicService.GetEntityByIdAsync(comicId);
            // var comic = await _comicService.GetEntityByIdAsync(3);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" });
            }
            Console.WriteLine("Comic Found : " + comic.Title);

            var cartItem = new ShoppingCartItem
            {
                ShoppingCartID = userCart.ShoppingCartID,
                ComicID = comic.ID,
                // ComicID = comicId,
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PriceAtAdd = comic.Price
            };
            Console.WriteLine("Shopping Cart Instance Created  ");
            await _cartItemService.CreateEntityAsync(cartItem);
            Console.WriteLine("Shopping Cart Created: " + userCart.ShoppingCartID);

            var allItems = await _cartItemService.GetAllEntityAsync();
            userCartItems = allItems.Where(i => i.ShoppingCartID == userCart.ShoppingCartID).ToList();

            var allComics = await _comicService.GetAllEntityAsync();

            // ✅ Debug log: print all items in this user's cart
            Console.WriteLine($"=== Cart Items for User ID: {appUser.Id} ===");
            foreach (var item in userCartItems)
            {
                var relatedComic = allComics.FirstOrDefault(c => c.ComicID == item.ComicID);
                Console.WriteLine($"ItemID: {item.ShoppingCartItemID}, Comic: {relatedComic?.Title}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}, Added: {item.CreatedAt}");
                // Console.WriteLine($"ItemID: {item.ShoppingCartItemID}, Comic: {comic.Title}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}, Added: {item.CreatedAt}");
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
                // ComicTitle = allComics.FirstOrDefault(c => c.ComicID == item.ComicID)?.Title,
                ComicTitle = allComics.FirstOrDefault(c => c.ID == item.ComicID)?.Title,
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

        // 3. Checkout and make a purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> Checkout()
        {
            Console.WriteLine("=== Checkout ===");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }

            // Get all carts and find the user's cart
            var allCarts = await _cartService.GetAllEntityAsync();
            var userCart = allCarts.FirstOrDefault(c => c.UserID == user.Id);

            if (userCart == null)
            {
                return BadRequest(new { message = "Shopping cart is empty" });
            }
            Console.WriteLine("User Cart ID: " + userCart.ShoppingCartID);

            // Fetch all cart items associated with the user's cart
            var allItems = await _cartItemService.GetAllEntityAsync();
            var userCartItems = allItems.Where(i => i.ShoppingCartID == userCart.ShoppingCartID).ToList();

            // Fetch all comics for calculating titles and prices
            var allComics = await _comicService.GetAllEntityAsync();

            // Calculate the total price of the cart
            decimal totalPrice = userCartItems.Sum(item => item.PriceAtAdd * item.Quantity);

            // Debug print cart details (optional)
            Console.WriteLine("=== Checkout ===");
            Console.WriteLine($"User ID: {user.Id}");
            Console.WriteLine($"Total Price: {totalPrice:C}");
            Console.WriteLine("Items in cart:");
            foreach (var item in userCartItems)
            {
                var relatedComic = allComics.FirstOrDefault(c => c.ComicID == item.ComicID);
                Console.WriteLine($"ItemID: {item.ShoppingCartItemID}, Comic: {relatedComic?.Title}, Quantity: {item.Quantity}, Price: {item.PriceAtAdd:C}");
            }
            Console.WriteLine("=== End of Cart ===");

            // Create an order
            var order = new Order
            {
                UserID = user.Id,
                OrderDate = DateTime.Now,
                OrderDetails = "Order for comics",
                TotalPrice = totalPrice,
                OrderItems = userCartItems.Select(item => new OrderItem
                {
                    ComicID = item.ComicID,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.PriceAtAdd
                }).ToList()
            };
            Console.WriteLine("Order Created: " + order.OrderID);


            await _orderService.CreateEntityAsync(order);

            Console.WriteLine("Creating Payment");
            // Create a payment
            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentDate = DateTime.Now,
                PaymentAmount = totalPrice,
                TransactionID = new Random().Next(100000, 999999) // Example transaction ID
            };

            await _paymentService.CreateEntityAsync(payment);
            Console.WriteLine("Payment Created: " + payment.PaymentID);

            // Clear the shopping cart after successful purchase
            foreach (var item in userCartItems)
            {
                await _cartItemService.DeleteEntityAsync(item.ShoppingCartItemID);
            }
            Console.WriteLine("Shopping cart cleared.");

            return Ok(new { message = "Purchase successful" });

        }

        // 4. View order history
        [HttpGet("orders")]
        public async Task<IActionResult> ViewOrders()
        {
            Console.WriteLine("=== View Orders ===");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }

            var allOrders = await _orderService.GetAllEntityAsync();
            var userOrders = allOrders.Where(o => o.UserID == user.Id).ToList();
            Console.WriteLine("User ID: " + user.Id);
            Console.WriteLine("User Orders: " + userOrders.Count);

            if (!userOrders.Any())
            {
                return NotFound(new { message = "No orders found for this user." });
            }
            Console.WriteLine("=== User Orders ===");
            foreach (var order in userOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderID}, Total Price: {order.TotalPrice:C}, Order Date: {order.OrderDate}");
                //ERORr
                // You can also print order items if needed
                // foreach (var item in order.OrderItems)
                // {
                //     Console.WriteLine("=== Order Items ===");
                //     // Console.WriteLine($"  Comic ID: {item.ComicID}, Quantity: {item.Quantity}, Price: {item.PriceAtPurchase:C}");
                // }
            }


            return Ok(userOrders); // Return user's orders as JSON
        }
        // 5. View order details
        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> ViewOrderDetails(int orderId)
        {
            Console.WriteLine("=== View Order Details ===");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }
            Console.WriteLine("=== View Order Details ===");
            Console.WriteLine("User ID: " + user.Id);
            Console.WriteLine("Order ID: " + orderId);
            // Get all orders and find the user's order

            // var allOrders = await _orderService.GetAllEntityAsync();
            var allOrders = await _orderItemService.GetAllEntityAsync();
            var order = allOrders.FirstOrDefault(o => o.OrderID == orderId );
            Console.WriteLine("Order Found: " + order?.OrderID);

            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }
            Console.WriteLine("Order ID: " + order.OrderID);
                var orderItems = await _orderItemService.GetAllEntityAsync();
                var itemsForOrder = orderItems.Where(oi => oi.OrderID == order.OrderID).ToList();

                Console.WriteLine("=== Order Items ===");
                foreach (var item in itemsForOrder)
                {
                    Console.WriteLine($"  Comic ID: {item.ComicID}, Quantity: {item.Quantity}, Price: {item.PriceAtPurchase:C}");
                }
                Console.WriteLine("=== End of Order Items ===");
//             Console.WriteLine("Total Price: " + order.TotalPrice);
//             Console.WriteLine("Order Date: " + order.OrderDate);
//             Console.WriteLine("Order Details: " + order.OrderDetails);
//             // You can also print order items if needed
// //             Console.WriteLine("Order Items:");   
// //            ERROR
//             foreach (var item in order.OrderItems)
//             {
//                 Console.WriteLine($"  Comic ID: {item.ComicID}, Quantity: {item.Quantity}, Price: {item.PriceAtPurchase:C}");
//             }   

            return Ok(order); // Return order details as JSON
        }
        // 6. View payment history
        [HttpGet("payments")]
        public async Task<IActionResult> ViewPayments()
        {

    Console.WriteLine("=== View Payments ===");

    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return Unauthorized(); // Ensure user is logged in
    }

    Console.WriteLine("User ID: " + user.Id);
    Console.WriteLine("User Name: " + user.UserName);

    var allPayments = await _paymentService.GetAllEntityAsync();
    Console.WriteLine("All Payments: " + allPayments.Count);
    Console.WriteLine("=== All Payments ===");
    var userPayments = allPayments
        .Where(p => p.OrderID != null) // Ensure the payment is associated with an order
        .Select(p => new
        {
            p.PaymentID,
            p.OrderID,
            p.PaymentAmount,
            PaymentDate = p.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"),
            p.TransactionID
        })
        .ToList();

    Console.WriteLine("=== User Payments ===");
    foreach (var payment in userPayments)
    {
        Console.WriteLine($"Payment ID: {payment.PaymentID}");
        Console.WriteLine($"Order ID: {payment.OrderID}");
        Console.WriteLine($"Payment Amount: {payment.PaymentAmount:C}");
        Console.WriteLine($"Payment Date: {payment.PaymentDate}");
        Console.WriteLine($"Transaction ID: {payment.TransactionID}");
        Console.WriteLine("---");
    }

    return Ok(userPayments);

        }

    }
}
