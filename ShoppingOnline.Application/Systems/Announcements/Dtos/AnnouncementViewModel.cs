using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Enum;

namespace ShoppingOnline.Application.Systems.Announcements.Dtos
{
    public class AnnouncementViewModel
    {
        public string Id { get; set; }

        [Required] [StringLength(250)] public string Title { set; get; }

        [StringLength(250)] public string Content { set; get; }

        public Guid? UserId { set; get; }

        public DateTime DateCreated { set; get; }

        public DateTime DateModified { set; get; }

        public Status Status { set; get; }
        
        public string Avatar { get; set; }

        public List<AnnouncementUserViewModel> AnnouncementUsers { get; set; }
    }
}