using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.EF.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using ShoppingOnline.Data.Entities.Content;

namespace ShoppingOnline.Data.EF.Configurations
{
    public class ContactDetailConfiguration : DbEntityConfiguration<Contact>
    {
        public override void Configure(EntityTypeBuilder<Contact> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasMaxLength(255).IsRequired();
            // etc.
        }
    }
}
