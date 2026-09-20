using backend.Services;
using System.Text.RegularExpressions;
using Xunit;

namespace backend.Tests
{
	public class OrderNumberGeneratorTests
	{
		private readonly OrderNumberGenerator _generator = new();
		private static readonly Regex Format = new(@"^\d{8}-[0-9A-F]{8}$", RegexOptions.Compiled);

		//Verifitation order number format
		[Fact]
		public void GenerateOrderNumber_MatchesExpectedFormat() =>
		Assert.Matches(Format, _generator.GenerateOrderNumber());

		//Order number uniqueness check
		[Fact]
		public void GenerateOrderNumber_ProducesDistinctValues()
		{
			var generated = Enumerable.Range(0, 1000)
				.Select(_ => _generator.GenerateOrderNumber());
			Assert.Equal(1000, generated.Distinct().Count());
		}

		//Order number date time - today's date
		[Fact]
		public void GenerateOrderNumber_StartsWithTodaysDate()
		{
			var number = _generator.GenerateOrderNumber();
			Assert.StartsWith(DateTime.Today.ToString("yyyyMMdd"), number);
		}
	}
}
