using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Application.ECommerce.Products.Dtos
{
    public class SizeViewModel
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Name
        {
            get; set;
        }
    }
}