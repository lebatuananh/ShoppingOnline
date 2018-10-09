using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.EF.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using ShoppingOnline.Data.Entities.Content;

namespace ShoppingOnline.Data.EF.Configurations
{
    public class TagConfiguration : DbEntityConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> entity)
        {
            entity.Property(c => c.Id).HasMaxLength(50)
                .IsRequired().IsUnicode(false).HasMaxLength(50);
        }
    }
}
