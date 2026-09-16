namespace backend.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public string SenderCity { get; set; } = String.Empty;
		public string SenderAddress { get; set; } = String.Empty;
		public string RecipientCity { get; set; } = String.Empty;
		public string RecipientAddress {  get; set; } = String.Empty;
		public decimal CargoWeight { get; set; }
		public DateTime PickupDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
