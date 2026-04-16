using System;
using System.Collections.Generic;

namespace TrendifyV1.ViewModels.UserOrderViewModels
{
    public class MyOrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }

    public class MyOrderDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public List<MyOrderItemViewModel> Items { get; set; } = new();
    }

    public class MyOrderItemViewModel
    {
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}