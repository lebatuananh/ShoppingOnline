using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Data.Interfaces;

namespace ShoppingOnline.Data.Entities.System
{
    [Table("AppUsers")]
    public class AppUser : IdentityUser<Guid>, IDateTracking, ISwitchable
    {
        private Guid _guid;

        public AppUser()
        {
        }

        public AppUser(Guid guid, string fullName, string userName, string email, string phoneNumber, string avatar,
            Status status, DateTime? birthday, bool gender)
        {
            this._guid = guid;
            this.FullName = fullName;
            this.UserName = userName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.Avatar = avatar;
            this.Status = status;
            this.BirthDay = BirthDay;
            this.Gender = gender;
        }

        public string FullName { get; set; }
        public DateTime? BirthDay { set; get; }
        public decimal Balance { get; set; }
        public string Avatar { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
    }
}