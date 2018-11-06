using System.Collections.Generic;

namespace ShoppingOnline.Application.Common.Advertisements.Dtos
{
    public class AdvertisementPageViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<AdvertisementPositionViewModel> AdvertisementPositions { get; set; }
    }
}