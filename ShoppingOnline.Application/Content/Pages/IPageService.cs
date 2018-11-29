using System;
using System.Collections.Generic;
using ShoppingOnline.Application.Content.Blogs.Dtos;
using ShoppingOnline.Application.Content.Pages.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Content.Pages
{
    public interface IPageService : IDisposable
    {
        void Add(PageViewModel pageVm);

        void Update(PageViewModel pageVm);

        void Delete(int id);

        List<PageViewModel> GetAll();

        PagedResult<PageViewModel> GetAllPaging(string keyword, int page, int pageSize);

        PageViewModel GetByAlias(string alias);

        PageViewModel GetById(int id);

        void SaveChanges();
    }
}