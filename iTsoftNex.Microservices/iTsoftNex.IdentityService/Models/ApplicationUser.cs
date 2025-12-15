using Microsoft.AspNetCore.Identity;

namespace iTsoftNex.IdentityService.Models
{
    // Extends the default IdentityUser to include custom POS-specific properties
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty; // Fix for non-nullable property
        public int StoreId { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
