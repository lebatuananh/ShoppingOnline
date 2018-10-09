using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.EF.Extensions;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.Entities.System;

namespace ShoppingOnline.Data.EF.Configurations
{
    internal class AnnouncementConfiguration : DbEntityConfiguration<Announcement>
    {
        public override void Configure(EntityTypeBuilder<Announcement> entity)
        {
            entity.HasKey(n => n.Id);
            entity.Property(c => c.Id).HasMaxLength(128).IsRequired();
            // etc.
        }
    }
}