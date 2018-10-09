using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Application.ECommerce.Products.Dtos
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public ProductViewModel Product { get; set; }

        [StringLength(250)] public string Path { get; set; }

        [StringLength(250)] public string Caption { get; set; }
    }
}