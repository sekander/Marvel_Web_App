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

        public ShoppingCart userCart;

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
        [HttpGet("comics")]
        public async Task<IActionResult> ViewComics()
        {
            var comics = await _comicService.GetAllEntityAsync();
            return Ok(comics); // Return a list of comics as JSON
        }

        // 2. Add comic to the shopping cart
        [HttpPost("shopping-cart/add/{comicId}")]
        public async Task<IActionResult> AddToCart(int comicId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }

            // ShoppingCart cart;
            // string userID =  

            // Check if shopping cart exists for the user
            // var cart = await _cartService.GetEntityByIdAsync(user.Id);
            // if (cart == null)
            // {
                // Create a new shopping cart if one doesn't exist
                // cart = new ShoppingCart
                
                userCart = new ShoppingCart
                {
                    UserID = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                // await _cartService.CreateEntityAsync(cart);
                await _cartService.CreateEntityAsync(userCart);
            // }

            // Fetch comic from the database
            var comic = await _comicService.GetEntityByIdAsync(comicId);
            if (comic == null)
            {
                return NotFound(new { message = "Comic not found" }); // Comic not found
            }

            // Add the comic to the shopping cart
            var cartItem = new ShoppingCartItem
            {
                // ShoppingCartID = cart.ShoppingCartID,
                ShoppingCartID = userCart.ShoppingCartID,
                ComicID = comic.ComicID,
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PriceAtAdd = comic.Price
            };

            await _cartItemService.CreateEntityAsync(cartItem);

            return Ok(new { message = "Comic added to shopping cart successfully" });
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
    }
}
