using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using ScwSvc.Models;
using static ScwSvc.Globals.DbConnectionString;

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
            services.AddDbContextPool<DbSysContext>(o => o.UseNpgsql($"Server={Server}; Port={Port}; Database=scw; User Id={SysUser}; Password={SysPass}; SearchPath=scw1_sys,public"));
            services.AddDbContextPool<DbDynContext>(o => o.UseNpgsql($"Server={Server}; Port={Port}; Database=scw; User Id={DynUser}; Password={DynPass}; SearchPath=scw1_dyn"));

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

#if DEBUG
            services.AddSwaggerGen(opts => opts.SwaggerDoc("v0", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Spreadsheet Components for Web-Based Projects API", Version = "v0" }));
            services.AddSwaggerGenNewtonsoftSupport();
            SetOutputFormatters(services);
#endif
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

            app.UseAuthentication();
            app.UseAuthorization();

#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v0/swagger.json", "Spreadsheet Components for Web-Based Projects API"));
#endif

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

        // Hack: works around compatibility issue between OData and Swagger until officially fixed
        private static void SetOutputFormatters(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                IEnumerable<ODataOutputFormatter> outputFormatters =
                    options.OutputFormatters.OfType<ODataOutputFormatter>()
                        .Where(formatter => formatter.SupportedMediaTypes.Count == 0);

                IEnumerable<ODataInputFormatter> inputFormatters =
                    options.InputFormatters.OfType<ODataInputFormatter>()
                        .Where(formatter => formatter.SupportedMediaTypes.Count == 0);

                foreach (var outputFormatter in outputFormatters)
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
                }

                foreach (var inputFormatter in inputFormatters)
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
                }
            });
        }
    }
}
