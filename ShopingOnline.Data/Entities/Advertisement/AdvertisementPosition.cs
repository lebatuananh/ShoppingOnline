using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingOnline.Infrastructure.SharedKernel;

namespace ShoppingOnline.Data.Entities.Advertisement
{
    [Table("AdvertisementPositions")]
    public class AdvertisementPosition : DomainEntity<string>
    {
        public AdvertisementPosition()
        {
        }

        public AdvertisementPosition(string pageId, string name)
        {
            PageId = pageId;
            Name = name;
        }

        [StringLength(20)] public string PageId { get; set; }

        [StringLength(250)] public string Name { get; set; }

        [ForeignKey("PageId")] public virtual AdvertisementPage AdvertisementPage { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}