namespace Ecommerce.Models
{
    public class CartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
