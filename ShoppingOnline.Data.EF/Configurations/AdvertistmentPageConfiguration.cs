using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingOnline.Data.EF.Extensions;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.Entities.Advertisement;

namespace ShoppingOnline.Data.EF.Configurations
{
    public class AdvertistmentPageConfiguration : DbEntityConfiguration<AdvertisementPage>
    {
        public override void Configure(EntityTypeBuilder<AdvertisementPage> entity)
        {
            entity.Property(c => c.Id).HasMaxLength(20).IsRequired();
            // etc.
        }
    }
}