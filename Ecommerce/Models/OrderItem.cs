using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; } 

        [Required]
        public int ProductId { get; set; } 

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; } 

        public decimal TotalPrice => Quantity * UnitPrice;
        
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public string? UserId { get; internal set; }
    }
}
