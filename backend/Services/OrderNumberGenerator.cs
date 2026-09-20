namespace backend.Services
{
	public class OrderNumberGenerator
	{
		public string GenerateOrderNumber()
		{
			return $"{DateTime.Today:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
		}
	}
}
