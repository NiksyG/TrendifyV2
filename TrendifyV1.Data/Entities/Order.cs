using System.ComponentModel.DataAnnotations;

namespace TrendifyV1.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public string Status { get; set; } = "Обработва се";

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}