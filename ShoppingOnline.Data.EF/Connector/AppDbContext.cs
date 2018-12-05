using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ShoppingOnline.Data.Entities;
using ShoppingOnline.Data.Interfaces;
using ShoppingOnline.Data.EF.Configurations;
using ShoppingOnline.Data.EF.Extensions;
using System;
using System.IO;
using System.Linq;
using ShoppingOnline.Data.Entities.Advertisement;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Infrastructure.SharedKernel;

namespace ShoppingOnline.Data.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Language> Languages { set; get; }

        public DbSet<SystemConfig> SystemConfigs { get; set; }

        public DbSet<Function> Functions { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<AppRole> AppRoles { get; set; }

        public DbSet<Announcement> Announcements { set; get; }

        public DbSet<AnnouncementUser> AnnouncementUsers { set; get; }

        public DbSet<Blog> Bills { set; get; }

        public DbSet<BillDetail> BillDetails { set; get; }

        public DbSet<Blog> Blogs { set; get; }

        public DbSet<BlogTag> BlogTags { set; get; }

        public DbSet<Color> Colors { set; get; }

        public DbSet<Contact> Contacts { set; get; }

        public DbSet<Feedback> Feedbacks { set; get; }

        public DbSet<Footer> Footers { set; get; }

        public DbSet<Page> Pages { set; get; }

        public DbSet<Product> Products { set; get; }

        public DbSet<ProductCategory> ProductCategories { set; get; }

        public DbSet<ProductImage> ProductImages { set; get; }

        public DbSet<ProductQuantity> ProductQuantities { set; get; }

        public DbSet<ProductTag> ProductTags { set; get; }

        public DbSet<Size> Sizes { set; get; }

        public DbSet<Slide> Slides { set; get; }

        public DbSet<Tag> Tags { set; get; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<WholePrice> WholePrices { get; set; }

        public DbSet<AdvertisementPage> AdvertistmentPages { get; set; }

        public DbSet<Advertisement> Advertistments { get; set; }

        public DbSet<AdvertisementPosition> AdvertistmentPositions { get; set; }

        public DbSet<Shipper> Shippers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Identity Config

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims")
                .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles")
                .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens")
               .HasKey(x => new { x.UserId });

            #endregion Identity Config

            builder.AddConfiguration(new TagConfiguration());

            builder.AddConfiguration(new BlogTagConfiguration());

            builder.AddConfiguration(new ContactDetailConfiguration());

            builder.AddConfiguration(new FooterConfiguration());

            builder.AddConfiguration(new PageConfiguration());

            builder.AddConfiguration(new FunctionConfiguration());

            builder.AddConfiguration(new ProductTagConfiguration());

            builder.AddConfiguration(new SystemConfigConfiguration());

            builder.AddConfiguration(new AdvertistmentPositionConfiguration());

            builder.AddConfiguration(new AdvertistmentPageConfiguration());

            builder.AddConfiguration(new AnnouncementConfiguration());

            //base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            try
            {
                var modified = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

                foreach (EntityEntry item in modified)
                {
                    var changedOrAddedItem = item.Entity as IDateTracking;
                    if (changedOrAddedItem != null)
                    {
                        if (item.State == EntityState.Added)
                        {
                            changedOrAddedItem.DateCreated = DateTime.Now;
                        }
                        changedOrAddedItem.DateModified = DateTime.Now;
                    }
                }
                return base.SaveChanges();
            }
            catch (DbUpdateException entityException)
            {

                throw new ModelValidationException(entityException.Message);
            }

        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();
                var builder = new DbContextOptionsBuilder<AppDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                builder.UseSqlServer(connectionString);
                return new AppDbContext(builder.Options);
            }
        }
    }
}