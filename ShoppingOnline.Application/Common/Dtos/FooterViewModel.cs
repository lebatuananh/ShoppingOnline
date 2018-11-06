using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Application.Common.Dtos
{
    public class FooterViewModel
    {
        public string Id { get; set; }

        [Required] public string Content { set; get; }
    }
}