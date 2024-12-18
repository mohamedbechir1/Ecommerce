using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderNumber { get; set; } 

        
        [Required]
        public string CustomerId { get; set; } 
        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }
		

		[Required]
        [Phone]
        public string PhoneNumber { get; set; }
		public User User { get; set; }

		[Required]
        public string PaymentMethod { get; set; } 

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "En traitement"; 

        public DateTime? DeliveryDate { get; set; } 

        
        public virtual ICollection<OrderItem> OrderItems { get; set; }
		public string? UserId { get; internal set; }

		public Order()
        {
            OrderItems = new List<OrderItem>();
        }
    }
}
