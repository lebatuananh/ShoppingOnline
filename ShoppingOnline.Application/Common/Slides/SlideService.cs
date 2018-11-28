using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;
using System.Linq;

namespace ShoppingOnline.Application.Common.Slides
{
    public class SlideService : ISlideService
    {
        private IRepository<Slide, int> _slideRepository;
        private IUnitOfWork _unitOfWork;

        public SlideService(IRepository<Slide, int> slideRepository, IUnitOfWork unitOfWork)
        {
            this._slideRepository = slideRepository;
            this._unitOfWork = unitOfWork;
        }

        public SlideViewModel Add(SlideViewModel viewModel)
        {
            var slide = Mapper.Map<SlideViewModel, Slide>(viewModel);
            _slideRepository.Add(slide);
            return viewModel;
        }

        public void Delete(int id)
        {
            _slideRepository.Remove(id);
        }

        public PagedResult<SlideViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _slideRepository.FindAll(x => x.Status);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();

            query = query.OrderByDescending(n => n.Id).OrderBy(n => n.DisplayOrder).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<SlideViewModel>().ToList();

            var paginationSet = new PagedResult<SlideViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public SlideViewModel GetById(int id)
        {
            var query = _slideRepository.FindById(id);
            var model = Mapper.Map<Slide, SlideViewModel>(query);
            return model;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(SlideViewModel viewModel)
        {
            var slide = Mapper.Map<SlideViewModel, Slide>(viewModel);
            _slideRepository.Update(slide);
        }
    }
}