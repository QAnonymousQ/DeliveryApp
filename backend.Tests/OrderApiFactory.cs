using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace backend.Tests
{
	public class OrderApiFactory:WebApplicationFactory<Program>
	{
		private readonly string _dbName = Guid.NewGuid().ToString(); // изоляция на тест

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseSetting("UseInMemoryDatabase", _dbName);
		}
	}

	internal static class TestHelpers
	{
		public static async Task SeedAsync(OrderApiFactory factory, params Order[] orders)
		{
			using var scope = factory.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await db.Database.EnsureCreatedAsync();
			db.Orders.AddRange(orders);
			await db.SaveChangesAsync();
		}
	}
}