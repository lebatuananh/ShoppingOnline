using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Application.Common.Dtos;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Application.Systems.Settings.Dtos;
using ShoppingOnline.Data.Entities.Advertisement;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Constants;

namespace ShoppingOnline.Application.Common
{
    public class CommonService : ICommonService
    {
        private IRepository<Footer, string> _footerRepository;
        private IRepository<Advertisement, int> _advertistmentRepository;

        private IRepository<SystemConfig, string> _systemConfigRepository;
        private IUnitOfWork _unitOfWork;
        private IRepository<Slide, int> _slideRepository;

        public CommonService(IRepository<Footer, string> footerRepository,
            IRepository<SystemConfig, string> systemConfigRepository,
            IUnitOfWork unitOfWork, IRepository<Slide, int> slideRepository,
            IRepository<Advertisement, int> advertistmentRepository)
        {
            this._footerRepository = footerRepository;
            this._systemConfigRepository = systemConfigRepository;
            this._unitOfWork = unitOfWork;
            this._slideRepository = slideRepository;
            this._advertistmentRepository = advertistmentRepository;
        }

        public AdvertisementViewModel GetAdvertistment()
        {
            var model = _advertistmentRepository.FindAll(n => n.Status == Status.Active).OrderBy(n => n.SortOrder)
                .Take(1).SingleOrDefault();
            return Mapper.Map<Advertisement, AdvertisementViewModel>(model);
        }

        public FooterViewModel GetFooter()
        {
            return Mapper.Map<Footer, FooterViewModel>(
                _footerRepository.FindSingle(x => x.Id == CommonConstants.DefaultFooterId));
        }

        public List<SlideViewModel> GetSlides(string groupAlias)
        {
            return _slideRepository.FindAll(x => x.Status && x.GroupAlias == groupAlias)
                .ProjectTo<SlideViewModel>().ToList();
        }

        public SystemConfigViewModel GetSystemConfig(string code)
        {
            return Mapper.Map<SystemConfig, SystemConfigViewModel>(
                _systemConfigRepository.FindSingle(x => x.Id == code));
        }
    }
}