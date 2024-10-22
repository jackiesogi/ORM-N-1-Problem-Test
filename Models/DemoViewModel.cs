using System.Collections.Generic;
using PersonalWebsite.Models;

namespace PersonalWebsite.Models
{
    public class DemoViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
