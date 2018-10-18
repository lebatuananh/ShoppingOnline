using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Data.Interfaces;
using ShoppingOnline.Infrastructure.SharedKernel;

namespace ShoppingOnline.Data.Entities.System
{
    [Table("Announcements")]
    public class Announcement : DomainEntity<string>, ISwitchable, IDateTracking
    {
        public Announcement()
        {
            AnnouncementUsers = new List<AnnouncementUser>();
        }

        public Announcement(string title, string content, Guid? userId, Status status)
        {
            Title = title;
            Content = content;
            UserId = userId;
            Status = status;
        }

        public Announcement(string title, string content, Status status)
        {
            Title = title;
            Content = content;
            Status = status;
        }

        [Required] [StringLength(250)] public string Title { set; get; }

        [StringLength(250)] public string Content { set; get; }

        public Guid? UserId { set; get; }
 
        [ForeignKey("UserId")] public virtual AppUser AppUser { get; set; }

        public virtual ICollection<AnnouncementUser> AnnouncementUsers { get; set; }
        public DateTime DateCreated { set; get; }
        public DateTime DateModified { set; get; }
        public Status Status { set; get; }
    }
}