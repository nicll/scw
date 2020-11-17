using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using ScwSvc.Models;

namespace ScwSvc
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
            services.AddControllers()
                .AddNewtonsoftJson();
            //services.AddDbContextPool<DbStoreContext>(o => o.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddOData();
            //services.AddODataQueryFilter();

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Cookie.Name = "ScwSession";
                    opts.Cookie.IsEssential = true;
                    opts.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    opts.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/api/service/unauthorized");
                    opts.LoginPath = new Microsoft.AspNetCore.Http.PathString("/api/service/unauthenticated");
                    opts.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/api/service/logout");
                    opts.ReturnUrlParameter = "from";
                    opts.ClaimsIssuer = "ScwSvc";
                    opts.SlidingExpiration = true;
                    opts.ExpireTimeSpan = TimeSpan.FromDays(1); // extend this later
                });
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
#if DEBUG
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
#endif
            {
                app.UseHsts();
                app.UseExceptionHandler("/error");
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //var defaultConventions = ODataRoutingConventions.CreateDefault();
                endpoints.EnableDependencyInjection();
                endpoints.MapControllers();
                endpoints.Select().Expand().Filter().OrderBy().Count().MaxTop(256);
                //endpoints.MapODataRoute("ODataRoute", "odata", GetEdmModel(app.ApplicationServices),
                //    new DefaultODataPathHandler(), defaultConventions.Except(defaultConventions.OfType<MetadataRoutingConvention>()));
            });
        }

        /*
        private IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<Student>("Students").EntityType.Select().Expand().Count().Filter().OrderBy();
            return builder.GetEdmModel();
        }
        */
    }
}
