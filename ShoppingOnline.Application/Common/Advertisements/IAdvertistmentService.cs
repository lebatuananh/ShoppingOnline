using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Utilities.Dtos;
using System.Collections.Generic;

namespace ShoppingOnline.Application.Common.Advertisements
{
    public interface IAdvertisementService
    {
        PagedResult<AdvertisementViewModel> GetAllPaging(string keyword, int page, int pageSize);

        AdvertisementViewModel Add(AdvertisementViewModel viewModel);

        void Update(AdvertisementViewModel viewModel);

        void Delete(int id);

        AdvertisementViewModel GetById(int id);

        void Save();

        List<AdvertisementPositionViewModel> GetAllAdvertisementPosition();
    }
}