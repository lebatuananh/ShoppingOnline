using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Data.Interfaces;
using ShoppingOnline.Infrastructure.SharedKernel;

namespace ShoppingOnline.Data.Entities.Advertisement
{
    [Table("Advertisements")]
    public class Advertisement : DomainEntity<int>, ISwitchable, IDateTracking
    {
        public Advertisement()
        {
        }

        public Advertisement(string name, string description, string image, string url, string positionId, Status status, DateTime dateCreated, DateTime dateModified, int sortOrder)
        {
            Name = name;
            Description = description;
            Image = image;
            Url = url;
            PositionId = positionId;
            Status = status;
            DateCreated = dateCreated;
            DateModified = dateModified;
            SortOrder = sortOrder;
        }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [StringLength(250)]
        public string Url { get; set; }

        [StringLength(20)]
        public string PositionId { get; set; }

        public Status Status { set; get; }

        public DateTime DateCreated { set; get; }

        public DateTime DateModified { set; get; }

        public int SortOrder { set; get; }

        [ForeignKey("PositionId")]
        public virtual AdvertisementPosition AdvertisementPosition { get; set; }
    }
}