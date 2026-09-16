using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
	public class AppDbContext: DbContext
	{
		public DbSet<Order> Orders { get; set; } = null!;
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Order>()
				.HasIndex(o => o.OrderNumber)
				.IsUnique();
		}
	}
}
