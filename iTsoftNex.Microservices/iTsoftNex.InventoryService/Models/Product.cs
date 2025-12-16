namespace iTsoftNex.InventoryService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty; // Stock Keeping Unit (Unique Identifier)
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Inventory and Pricing Core Data
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }

        // POS-specific fields
        public string Category { get; set; } = string.Empty;
        public int StoreId { get; set; } // For multi-store management

        // Auditing
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; }
    }
}