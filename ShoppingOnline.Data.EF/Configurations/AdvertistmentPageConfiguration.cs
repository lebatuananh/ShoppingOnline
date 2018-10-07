using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.EF.Extensions;
using ShoppingOnline.Data.Entities;

namespace ShoppingOnline.Data.EF.Configurations
{
    public class AdvertistmentPageConfiguration : DbEntityConfiguration<AdvertistmentPage>
    {
        public override void Configure(EntityTypeBuilder<AdvertistmentPage> entity)
        {
            entity.Property(c => c.Id).HasMaxLength(20).IsRequired();
            // etc.
        }
    }
}