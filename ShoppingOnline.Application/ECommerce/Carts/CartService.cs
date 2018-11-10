using AutoMapper;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Infrastructure.Interfaces;

namespace ShoppingOnline.Application.ECommerce.Carts
{
    public class CartService : ICartService
    {
        private readonly IRepository<Color, int> _colorRepository;
        private readonly IRepository<Size, int> _sizeRepository;

        public CartService(IRepository<Color, int> colorRepository, IRepository<Size, int> sizeRepository)
        {
            this._colorRepository = colorRepository;
            this._sizeRepository = sizeRepository;
        }

        public ColorViewModel GetColor(int colorId)
        {
            return Mapper.Map<Color, ColorViewModel>(_colorRepository.FindById(colorId));
        }

        public SizeViewModel GetSize(int sizeId)
        {
            return Mapper.Map<Size, SizeViewModel>(_sizeRepository.FindById(sizeId));
        }
    }
}