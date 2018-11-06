using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Application.Content.Dtos;

namespace ShoppingOnline.Application.Content.Blogs.Dtos
{
    public class BlogTagViewModel
    {
        
        public int Id { get; set; }

        public int BlogId { set; get; }

        [MaxLength(50)]
        public string TagId { set; get; }

        public virtual BlogViewModel Blog { set; get; }

        public virtual TagViewModel Tag { set; get; }
    }
}