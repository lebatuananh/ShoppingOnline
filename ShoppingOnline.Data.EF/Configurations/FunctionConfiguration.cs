using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.EF.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using ShoppingOnline.Data.Entities.System;

namespace ShoppingOnline.Data.EF.Configurations
{
    public class FunctionConfiguration : DbEntityConfiguration<Function>
    {
        public override void Configure(EntityTypeBuilder<Function> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).IsRequired()
                .HasMaxLength(128).IsUnicode(false);
            // etc.
        }
    }
}
