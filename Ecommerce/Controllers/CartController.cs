using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceDbContext _context;

        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Carts
                .Where(c => c.UserId == int.Parse(userId))
                .Include(c => c.Product)
                .ToList();

            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.UserId == int.Parse(userId) && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _context.SaveChanges();
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = int.Parse(userId),
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.Carts.Add(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Cart");
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartItemId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Cart");
        }

        // POST: /Cart/UpdateCart
        [HttpPost]
        public IActionResult UpdateCart(int quantity, int cartItemId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Invalid quantity.";
                return RedirectToAction("Index", "Cart");
            }

            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartItemId && c.UserId == int.Parse(userId));

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "Item not found.";
            }

            return RedirectToAction("Index", "Cart");
        }

        // GET: /Cart/Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            
            var cartItems = _context.Carts
                .Where(c => c.UserId == int.Parse(userId))
                .Include(c => c.Product)
                .ToList();

            
            var total = cartItems.Sum(c => c.Product.Price * c.Quantity);
            ViewBag.Total = total;

            
            var checkoutModel = new CheckoutViewModel
            {
                CustomerId = user.Id.ToString(),
                CustomerName = user.Name,
               
                PaymentMethod = "Card"  
            };

            return View(checkoutModel);
        }
        [HttpPost]
        public IActionResult ConfirmOrder(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var order = new Order
                {
                    OrderNumber = "ORD-" + DateTime.Now.Ticks.ToString(),
                    CustomerId = model.CustomerId,
                    CustomerName = model.CustomerName,
                    DeliveryAddress = model.DeliveryAddress,
                    PhoneNumber = model.PhoneNumber,
                    PaymentMethod = model.PaymentMethod,
                    OrderDate = DateTime.Now,
                    Status = "En traitement"
                };

                
                _context.Orders.Add(order);
                _context.SaveChanges();

                
                var cartItems = _context.Carts
                    .Where(c => c.UserId == int.Parse(model.CustomerId))
                    .ToList();

                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        OrderId = order.Id,
                        UnitPrice = cartItem.Product.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                
                _context.Carts.RemoveRange(cartItems);
                _context.SaveChanges();

                
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            
            return View("Checkout", model);
        }

    }
}
