using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string PaymentMethod { get; set; }
    }
}
