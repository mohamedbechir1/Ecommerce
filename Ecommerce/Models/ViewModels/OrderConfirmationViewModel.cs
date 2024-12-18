namespace Ecommerce.Models.ViewModels
{
	public class OrderConfirmationViewModel
	{
		public int OrderId { get; set; }
		public string CustomerName { get; set; }
		public string DeliveryAddress { get; set; }
		public string PhoneNumber { get; set; }
		public decimal TotalAmount { get; set; }
	}
}
