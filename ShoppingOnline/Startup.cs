using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using ShoppignOnline.Application.Dapper.Implementations;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.Application.Common;
using ShoppingOnline.Application.Common.Advertisements;
using ShoppingOnline.Application.Common.Contacts;
using ShoppingOnline.Application.Common.Feedbacks;
using ShoppingOnline.Application.Common.Slides;
using ShoppingOnline.Application.Content.Blogs;
using ShoppingOnline.Application.Content.Pages;
using ShoppingOnline.Application.ECommerce.Bills;
using ShoppingOnline.Application.ECommerce.Carts;
using ShoppingOnline.Application.ECommerce.ProductCategories;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.Application.Systems.Announcements;
using ShoppingOnline.Application.Systems.Functions;
using ShoppingOnline.Application.Systems.Roles;
using ShoppingOnline.Application.Systems.Users;
using ShoppingOnline.Data.EF;
using ShoppingOnline.Data.EF.Abstract;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.WebApplication.Authorization;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.Helpers;
using ShoppingOnline.WebApplication.Services;
using ShoppingOnline.WebApplication.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShoppingOnline.Application.Systems.Shippers;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.DataProtects;

namespace ShoppingOnline.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.MigrationsAssembly("ShoppingOnline.Data.EF")));

            // Configure Identity
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddEmailConfirmTokenProvider();
            ;

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;

                //Email confirmation token
                options.Tokens.EmailConfirmationTokenProvider = SystemConstants.TokenProvider.EmailConfirm;
            });

            //Add Authentication
            services.AddAuthentication();

            // Add application services.
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            //DbInitializer
            services.AddTransient<DbInitializer>();

            //AutoMapper
            services.AddAutoMapper();
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp =>
                new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

            //MemoryCache
            services.AddMemoryCache();

            //Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
            });
            //MinResponse
            services.AddMinResponse();

            //Mvc
            //services.AddMvc(options =>
            //{
            //    options.CacheProfiles.Add("Default",
            //        new CacheProfile()
            //        {
            //            Duration = 60
            //        });
            //    options.CacheProfiles.Add("Never",
            //        new CacheProfile()
            //        {
            //            Location = ResponseCacheLocation.None,
            //            NoStore = true
            //        });
            //}).AddViewLocalization(
            //        LanguageViewLocationExpanderFormat.Suffix,
            //        opts => { opts.ResourcesPath = "Resources"; })
            //    .AddDataAnnotationsLocalization()
            //    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //Repository And UnitOfWork
            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepository<,>), typeof(EFRepository<,>));

            //Service
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAppUserService, AppUserService>();
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<IAnnouncementService, AnnouncementService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<IAdvertisementService, AdvertisementService>();
            services.AddTransient<IColorDapperService, ColorDapperService>();
            services.AddTransient<ISizeDapperService, SizeDapperService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<ISlideService, SlideService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IShipperService, ShipperService>();

            services.AddTransient<IEmailSender, EmailSender>();

            //Principal
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();

            //Authorization
            services.AddTransient<IAuthorizationHandler, BaseResourceAuthorizationHandler>();

            //SignalIR
            services.AddSignalR();

            //Config login Authen
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "ShoppingOnline.PO";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/admin/login";
            });

//            services.ConfigureApplicationCookie(options =>
//            {
//                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//                options.Cookie.Name = "ShoppingOnline.PO";
//                options.Cookie.HttpOnly = true;
//                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//                options.LoginPath = "/Identity/Account/Login";
//                // ReturnUrlParameter requires 
//                //using Microsoft.AspNetCore.Authentication.Cookies;
//                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
//                options.SlidingExpiration = true;
//            });
            // enables immediate logout, after updating the user's stat.
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            //lifespan for token:
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(2);
            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //Facebook, Google
            services.AddAuthentication().AddFacebook(n =>
            {
                n.AppId = Configuration["Authentication:Facebook:AppId"];
                n.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(n =>
            {
                n.ClientId = Configuration["Authentication:Google:ClientId"];
                n.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            //Recaptcha
            services.AddRecaptcha(new RecaptchaOptions()
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SerectKey"]
            });

            //Cache and MultiLanguage
            services.AddMvc();
            services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default",
                        new CacheProfile()
                        {
                            Duration = 60
                        });
                    options.CacheProfiles.Add("Never",
                        new CacheProfile()
                        {
                            Location = ResponseCacheLocation.None,
                            NoStore = true
                        });
                }).AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("vi-VN")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en-US");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/logfile-{Date}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseSession();
            app.UseMinResponse();

            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chatHub"); });

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseMvc(routes =>
            {
                //webapp
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                //admin
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}