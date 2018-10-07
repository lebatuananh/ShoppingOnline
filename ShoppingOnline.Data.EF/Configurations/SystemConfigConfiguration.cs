using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.EF.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingOnline.Data.EF.Configurations
{
    class SystemConfigConfiguration : DbEntityConfiguration<SystemConfig>
    {
        public override void Configure(EntityTypeBuilder<SystemConfig> entity)
        {
            entity.Property(c => c.Id).HasMaxLength(255).IsRequired();
            // etc.
        }
    }
}
