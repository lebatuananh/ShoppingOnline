using ShoppingOnline.Application.ECommerce.Products.Dtos;

namespace ShoppingOnline.WebApplication.Models
{
    public class ShoppingCartViewModel
    {
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ColorViewModel Color { get; set; }
        public SizeViewModel Size { get; set; }
    }
}