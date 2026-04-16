using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendifyV1.ViewModels.OrderAdminViewModels
{
    public class OrderListAdminViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }

    public class OrderDetailsAdminViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public List<OrderItemAdminViewModel> Items { get; set; } = new();
    }

    public class OrderItemAdminViewModel
    {
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
