using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Models;

namespace backend.Tests
{
	public class ApiTests
	{
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		private static OrderRequest ValidRequest() => new()
		{
			SenderCity = "Москва",
			SenderAddress = "ул. Ленина, 1",
			RecipientCity = "Санкт-Петербург",
			RecipientAddress = "Невский пр., 10",
			CargoWeight = 12.5m,
			PickupDate = DateTime.Today.AddDays(1)
		};

		private static async Task<Order> CreateOrderAsync(OrderApiFactory factory, OrderRequest request)
		{
			var client = factory.CreateClient();
			var resp = await client.PostAsJsonAsync("/api/orders", request);
			resp.EnsureSuccessStatusCode();
			return JsonSerializer.Deserialize<Order>(await resp.Content.ReadAsStringAsync(), JsonOptions)!;
		}

		[Fact]
		public async Task Create_ValidOrder_Returns201WithLocationAndOrderFields()
		{
			using var factory = new OrderApiFactory();
			var resp = await factory.CreateClient().PostAsJsonAsync("/api/orders", ValidRequest());

			Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
			Assert.NotNull(resp.Headers.Location);

			var order = JsonSerializer.Deserialize<Order>(await resp.Content.ReadAsStringAsync(), JsonOptions)!;
			Assert.True(order.Id > 0);
			Assert.NotEmpty(order.OrderNumber);
			Assert.Equal("Москва", order.SenderCity);
			Assert.Equal("Невский пр., 10", order.RecipientAddress);
			Assert.Equal(12.5m, order.CargoWeight);
		}

		[Fact]
		public async Task Create_PickupDateInPast_Returns400()
		{
			using var factory = new OrderApiFactory();
			var request = ValidRequest();
			request.PickupDate = new DateTime(2020, 1, 1);

			var resp = await factory.CreateClient().PostAsJsonAsync("/api/orders", request);

			Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
			var body = await resp.Content.ReadAsStringAsync();
			Assert.Contains("Дата забора не может быть в прошлом", body);
		}

		[Fact]
		public async Task Create_ZeroWeight_Returns400()
		{
			using var factory = new OrderApiFactory();
			var request = ValidRequest();
			request.CargoWeight = 0;

			var resp = await factory.CreateClient().PostAsJsonAsync("/api/orders", request);

			Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
		}

		[Fact]
		public async Task Create_EmptyRequiredField_Returns400()
		{
			using var factory = new OrderApiFactory();
			var request = ValidRequest();
			request.SenderCity = "";

			var resp = await factory.CreateClient().PostAsJsonAsync("/api/orders", request);

			Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
		}

		[Fact]
		public async Task Get_Orders_AreSortedByCreatedAtDescending()
		{
			using var factory = new OrderApiFactory();
			await TestHelpers.SeedAsync(factory,
				new Order
				{
					OrderNumber = "20260101-AAAA0001",
					SenderCity = "A1", SenderAddress = "a",
					RecipientCity = "B1", RecipientAddress = "b",
					CargoWeight = 1m, PickupDate = DateTime.Today,
					CreatedAt = new DateTime(2026, 1, 1)
				},
				new Order
				{
					OrderNumber = "20260102-BBBB0002",
					SenderCity = "A2", SenderAddress = "a",
					RecipientCity = "B2", RecipientAddress = "b",
					CargoWeight = 2m, PickupDate = DateTime.Today,
					CreatedAt = new DateTime(2026, 1, 2)
				});

			var resp = await factory.CreateClient().GetAsync("/api/orders");
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

			var orders = JsonSerializer.Deserialize<List<Order>>(await resp.Content.ReadAsStringAsync(), JsonOptions)!;
			Assert.Collection(orders,
				o => Assert.Equal("20260102-BBBB0002", o.OrderNumber),
				o => Assert.Equal("20260101-AAAA0001", o.OrderNumber));
		}

		[Fact]
		public async Task GetById_ExistingOrder_Returns200()
		{
			using var factory = new OrderApiFactory();
			var created = await CreateOrderAsync(factory, ValidRequest());

			var resp = await factory.CreateClient().GetAsync($"/api/orders/{created.Id}");

			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			var order = JsonSerializer.Deserialize<Order>(await resp.Content.ReadAsStringAsync(), JsonOptions)!;
			Assert.Equal(created.Id, order.Id);
		}

		[Fact]
		public async Task GetById_MissingOrder_Returns404()
		{
			using var factory = new OrderApiFactory();

			var resp = await factory.CreateClient().GetAsync("/api/orders/99999");

			Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
		}

		[Fact]
		public async Task Delete_RemovesOrder_AndSecondDeleteReturns404()
		{
			using var factory = new OrderApiFactory();
			var created = await CreateOrderAsync(factory, ValidRequest());
			var client = factory.CreateClient();

			var delete = await client.DeleteAsync($"/api/orders/{created.Id}");
			Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

			var getAfter = await client.GetAsync($"/api/orders/{created.Id}");
			Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);

			var deleteAgain = await client.DeleteAsync($"/api/orders/{created.Id}");
			Assert.Equal(HttpStatusCode.NotFound, deleteAgain.StatusCode);
		}
	}
}