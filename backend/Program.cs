

using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DB Registration
var inMemoryDatabaseName = builder.Configuration["UseInMemoryDatabase"];
if (!string.IsNullOrEmpty(inMemoryDatabaseName))
{
	builder.Services.AddDbContext<AppDbContext>(options => 
		options.UseInMemoryDatabase(inMemoryDatabaseName));
}
else
{
	builder.Services.AddDbContext<AppDbContext>(options => 
		options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
}

//OrderNumberGenerator registration
builder.Services.AddScoped<OrderNumberGenerator>();

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
	if (db.Database.IsSqlite()) 
	{
		db.Database.Migrate();
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}


app.UseCors("Frontend");

if (!app.Environment.IsDevelopment()) 
{
	app.UseHttpsRedirection();
}


app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{

}
