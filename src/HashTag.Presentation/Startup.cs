using System;
using System.IO;
using HashTag.Data;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Bootstrappers;
using HashTag.Infrastructure.Extensions;
using HashTag.Infrastructure.Filters;
using HashTag.Presentation.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HashTag.Presentation
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder.AddUserSecrets("HashTagSecrets");

            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                setup => setup.UseSqlServer(Configuration["db:default"], options => { options.MigrationsAssembly("HashTag.Presentation"); }));

            services.AddIdentity<ApplicationUser, ApplicationRole>(
                    setup =>
                    {
                        setup.Password.RequiredLength = 6;
                        setup.Password.RequireLowercase = false;
                        setup.Password.RequireUppercase = false;
                        setup.Password.RequireDigit = false;
                        setup.Password.RequireNonAlphanumeric = false;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(
                options => { options.AddPolicy("admin", policy => policy.RequireRole("admin")); });

            var authenticationBuilder = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            authenticationBuilder.AddCookie(setup =>
            {
                setup.Cookie.Name = "_HashTagAuth";
                setup.ExpireTimeSpan = TimeSpan.FromDays(1);
                setup.SlidingExpiration = true;
                setup.AccessDeniedPath = "/error/403";
            });

            if (!string.IsNullOrEmpty(Configuration["authentication:facebook:appId"]))
                authenticationBuilder.AddFacebook(setup =>
                {
                    setup.AppId = Configuration["authentication:facebook:appId"];
                    setup.AppSecret = Configuration["authentication:facebook:appSecret"];
                });

            if (!string.IsNullOrEmpty(Configuration["authentication:google:clientId"]))
                authenticationBuilder.AddGoogle(setup =>
                {
                    setup.ClientId = Configuration["authentication:google:clientId"];
                    setup.ClientSecret = Configuration["authentication:google:clientSecret"];
                });

            services.AddSession();

            services.AddMvc(
                    options =>
                    {
                        if (bool.Parse(Configuration["config:requireHttpsFilterEnabled"]))
                            options.Filters.Add(new RequireHttpsAttribute { Permanent = true });

                        options.Filters.Add(typeof(ApplicationExceptionFilter));

                        if (bool.Parse(Configuration["config:historyLogsEnabled"]))
                            options.Filters.Add(typeof(RequestHistoryLogFilterAttribute));

                        options.Filters.Add(typeof(UnitOfWorkFilter));
                    })
                .AddJsonOptions(
                    option =>
                    {
                        option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        option.SerializerSettings.Formatting = Formatting.Indented;
                        option.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });

            services.AddSingleton<IConfiguration>(Configuration);
            services.TryAddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<MessagesOptions>(Configuration.GetSection("messages"));

            ApplicationBootstrapper.ApplyAllBootstrappers(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSession();
            app.ConfigureNLog(env, loggerFactory, Configuration["db:default"]);

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "clientapp")),
                RequestPath = new PathString("/clientapp")
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}