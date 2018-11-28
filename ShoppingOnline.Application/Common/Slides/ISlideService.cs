using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Common.Slides
{
    public interface ISlideService
    {
        PagedResult<SlideViewModel> GetAllPaging(string keyword, int page, int pageSize);

        SlideViewModel Add(SlideViewModel viewModel);

        void Update(SlideViewModel viewModel);

        void Delete(int id);

        SlideViewModel GetById(int id);

        void Save();
    }
}