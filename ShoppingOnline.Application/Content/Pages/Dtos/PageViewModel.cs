using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Data.Enum;

namespace ShoppingOnline.Application.Content.Pages.Dtos
{
    public class PageViewModel
    {
        public int Id { set; get; }

        [Required]
        [MaxLength(256)]
        public string Name { set; get; }

        [MaxLength(256)]
        [Required]
        public string Alias { set; get; }

        public string Content { set; get; }
        public Status Status { set; get; }
    }
}