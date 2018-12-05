using System.Collections.Generic;
using ShoppingOnline.Application.Systems.Shippers.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Shippers
{
    public interface IShipperService
    {
        PagedResult<ShipperViewModel> GetAllPaging(string keyword, int page, int pageSize);

        List<ShipperViewModel> GetAll();

        ShipperViewModel Add(ShipperViewModel viewModel);

        void Update(ShipperViewModel viewModel);

        void Delete(int id);

        ShipperViewModel GetById(int id);

        void Save();
    }
}