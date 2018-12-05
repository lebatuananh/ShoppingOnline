using System.Collections.Generic;
using ShoppingOnline.Application.Content.Blogs.Dtos;


namespace ShoppingOnline.WebApplication.Models
{
    public class BlogDetailViewModel
    {
        public BlogViewModel Blog { get; set; }
        public List<BlogViewModel> MostBlogs { get; set; }
    }
}