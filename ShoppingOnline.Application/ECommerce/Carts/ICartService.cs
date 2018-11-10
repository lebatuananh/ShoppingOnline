using ShoppingOnline.Application.ECommerce.Products.Dtos;

namespace ShoppingOnline.Application.ECommerce.Carts
{
    public interface ICartService
    {
        ColorViewModel GetColor(int colorId);

        SizeViewModel GetSize(int sizeId);
    }
}