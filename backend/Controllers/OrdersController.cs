using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/orders")]
	public class OrdersController: ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly OrderNumberGenerator _numberGenerator;

		public OrdersController(AppDbContext db, OrderNumberGenerator numberGenerator)
		{
			_db = db;
			_numberGenerator = numberGenerator;
		}
		[HttpGet]
		public async Task<ActionResult<List<Order>>> Get() 
		{
			var orders = await _db.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();
			return Ok(orders);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetById(int id) 
		{
			var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(o=>o.Id==id);
			return order is null ? NotFound() : Ok(order); 
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id) 
		{
			var order = await _db.Orders.FirstOrDefaultAsync(o=>o.Id==id);
			if (order is null) return NotFound();
			
			_db.Orders.Remove(order);
			await _db.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost]
		public async Task<ActionResult<Order>> Create(OrderRequest request) 
		{
			if (request.PickupDate.Date< DateTime.Today) 
			{
				return  BadRequest(new {error= "Дата забора не может быть в прошлом" });
			}

			var order = new Order
			{
				OrderNumber = _numberGenerator.GenerateOrderNumber(),
				SenderCity = request.SenderCity,
				SenderAddress = request.SenderAddress,
				RecipientCity = request.RecipientCity,
				RecipientAddress = request.RecipientAddress,
				CargoWeight = request.CargoWeight,
				PickupDate = request.PickupDate
			};

			_db.Orders.Add(order);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
		}
		/*
		private string GenerateOrderNumber() 
		{
			return $"{DateTime.Today:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
		}
		*/
	}
}
