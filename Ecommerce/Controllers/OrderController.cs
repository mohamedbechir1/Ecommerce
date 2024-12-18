using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Ecommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly EcommerceDbContext _context;

        public OrderController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: Order/ConfirmOrder
        public IActionResult ConfirmOrder()
        {
            var model = new CheckoutViewModel();
            
            return View(model);
        }

        // POST: Order/ConfirmOrder
        [HttpPost]
        public IActionResult ConfirmOrder(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    DeliveryAddress = model.DeliveryAddress,
                    PhoneNumber = model.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };

                
                foreach (var cartItem in _context.OrderItems.Where(c => c.UserId == User.Identity.Name)) // Assuming cart is stored by user
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    });
                }

                
                _context.Orders.Add(order);
                _context.SaveChanges();

                
                var cartItems = _context.OrderItems.Where(c => c.UserId == User.Identity.Name).ToList();
                _context.OrderItems.RemoveRange(cartItems);
                _context.SaveChanges();

                
                return RedirectToAction("OrderConfirmation","Order", new { orderId = order.Id });
            }

            
            return View(model);
        }


        // GET: Order/OrderConfirmation
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.Id == orderId)
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderConfirmationViewModel
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
            };

            return View(model);
        }
		public IActionResult OrderHistory()
		{
			
			var orders = _context.Orders.Where(o => o.UserId == User.Identity.Name).ToList(); 
			return View(orders);  
		}
	}
}
