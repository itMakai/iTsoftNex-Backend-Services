using iTsoftNex.InventoryService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Database Configuration ---
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. Standard Service Setup ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Enabled by Swashbuckle.AspNetCore

var app = builder.Build();

// --- 3. Application Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Inventory is a protected resource, but we'll add the necessary
// UseAuthentication/UseAuthorization when we configure JWT validation later.
app.UseAuthorization();
app.MapControllers();

// Ensure the database is created and migrated on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    context.Database.Migrate();
}

app.Run();