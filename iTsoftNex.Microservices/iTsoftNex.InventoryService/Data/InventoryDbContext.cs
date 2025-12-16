using iTsoftNex.InventoryService.Models;
using Microsoft.EntityFrameworkCore;


namespace iTsoftNex.InventoryService.Data
{
    // Uses Primary Constructor
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure SKU is unique, critical for inventory lookup
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique();
        }
    }
}