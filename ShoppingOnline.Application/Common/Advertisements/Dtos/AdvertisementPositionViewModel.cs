using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Application.Common.Advertisements.Dtos
{
    public class AdvertisementPositionViewModel
    {
        public string Id { get; set; }

        [StringLength(20)] public string PageId { get; set; }

        [StringLength(250)] public string Name { get; set; }

        public AdvertisementPageViewModel AdvertisementPage { get; set; }

        public List<AdvertisementViewModel> Advertisements { get; set; }
    }
}