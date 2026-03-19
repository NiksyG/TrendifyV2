using Microsoft.AspNetCore.Identity;

namespace TrendifyV1.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<Order> Orders { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; }
    }
}
