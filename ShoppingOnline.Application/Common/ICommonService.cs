using System.Collections.Generic;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Application.Common.Dtos;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Application.Systems.Settings.Dtos;

namespace ShoppingOnline.Application.Common
{
    public interface ICommonService
    {
        FooterViewModel GetFooter();

        List<SlideViewModel> GetSlides(string groupAlias);

        AdvertisementViewModel GetAdvertistment();
        
        SystemConfigViewModel GetSystemConfig(string code);
    }
}