

using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DB Registration
builder.Services.AddDbContext<AppDbContext>(options => 
options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

//CORS for React
builder.Services.AddCors(options =>
options.AddPolicy("Frontend", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//Creating(Migrating) DB
using (var scope = app.Services.CreateScope()) 
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();

	if (!db.Orders.Any())
	{
		db.Orders.AddRange(
			new Order
			{
				OrderNumber = "ORDED-20260916-0001",
				SenderCity = "Москва",
				SenderAddress = "ул. Ленина, 1",
				RecipientCity = "СПб",
				RecipientAddress = "Невский пр., 10",
				CargoWeight = 15.5m,
				PickupDate = DateTime.Today.AddDays(1)
			},
			new Order
			{
				OrderNumber = "ORDER-20260916-0002",
				SenderCity = "Казань",
				SenderAddress = "ул. Баумана, 5",
				RecipientCity = "Москва",
				RecipientAddress = "ул. Тверская, 20",
				CargoWeight = 7.2m,
				PickupDate = DateTime.Today.AddDays(2)
			});
		db.SaveChanges();
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}


app.UseCors("Frontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
