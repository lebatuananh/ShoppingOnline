using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Application.Systems.Announcements.Dtos
{
    public class AnnouncementUserViewModel
    {
        public int Id { set; get; }

        [StringLength(128)]
        [Required]
        public string AnnouncementId { get; set; }

        public Guid UserId { get; set; }

        public bool? HasRead { get; set; }
    }
}