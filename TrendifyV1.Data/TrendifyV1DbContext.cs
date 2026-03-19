using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TrendifyV1.Data.Entities
{
    public class TrendifyV1DbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public TrendifyV1DbContext(DbContextOptions<TrendifyV1DbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}