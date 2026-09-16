using System.ComponentModel.DataAnnotations;
namespace backend.Models
{
	public class OrderRequest
	{
		[Required]
		public string SenderCity { get; set; } = String.Empty;
		[Required]
		public string SenderAddress { get; set; } = String.Empty;
		[Required]
		public string RecipientCity { get; set; } = String.Empty;
		[Required]
		public string RecipientAddress { get; set; } = String.Empty;
		[Range(0.001,double.MaxValue,ErrorMessage ="Вес груза должен быть больше 0")]
		public decimal CargoWeight { get; set; }
		[Required]
		public DateTime PickupDate { get; set; }
	}
}
