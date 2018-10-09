using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Application.Content.Dtos;

namespace ShoppingOnline.Application.ECommerce.Products.Dtos
{
    public class ProductTagViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [StringLength(50)]
        public string TagId { get; set; }

        public ProductViewModel Product { get; set; }

        public TagViewModel Tag { get; set; }
    }
}