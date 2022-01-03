using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScwSvc.DataAccess.Impl;
using ScwSvc.DataAccess.Interfaces;
using ScwSvc.Models;
using ScwSvc.Operations.Impl;
using ScwSvc.Operations.Interfaces;
using ScwSvc.Procedures.Impl;
using ScwSvc.Procedures.Interfaces;
using static ScwSvc.Globals.Authorization;
using static ScwSvc.Globals.CorsConfig;
using static ScwSvc.Globals.DbConnectionString;

namespace ScwSvc;

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
        services.Configure<KestrelServerOptions>(o => o.AddServerHeader = false);

        services.AddControllers();

        services.AddAutoMapper(config => config.AddProfile<AutoMapperProfile>());

        services.AddDbContextPool<DbSysContext>(o => o.UseNpgsql($"Server={Server}; Port={Port}; Database=scw; User Id={SysUser}; Password={SysPass}; SearchPath=scw1_sys,public").UseLazyLoadingProxies());
        services.AddDbContextPool<DbDynContext>(o => o.UseNpgsql($"Server={Server}; Port={Port}; Database=scw; User Id={DynUser}; Password={DynPass}; SearchPath=scw1_dyn"));
        services.AddScoped<ISysDbRepository, SysDbRepository>();
        services.AddScoped<IDynDbRepository, DynDbRepository>();
        services.AddScoped<IUserOperations, UserOperations>();
        services.AddScoped<ITableOperations, TableOperations>();
        services.AddScoped<IUserProcedures, UserProcedures>();
        services.AddScoped<IAdminProcedures, AdminProcedures>();
        services.AddScoped<IServiceProcedures, ServiceProcedures>();
        services.AddScoped<IGraphQLTableProcedures, GraphQLTableProcedures>();

        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<GraphQL.AdminQuery>();

        services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opts =>
            {
                opts.Cookie.Name = "ScwSession";
                opts.Cookie.IsEssential = true;
                opts.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                opts.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                opts.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/api/service/unauthorized");
                opts.LoginPath = new Microsoft.AspNetCore.Http.PathString("/api/service/unauthenticated");
                opts.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/api/service/logout");
                opts.ReturnUrlParameter = "from";
                opts.ClaimsIssuer = "ScwSvc";
                opts.SlidingExpiration = true;
                opts.ExpireTimeSpan = TimeSpan.FromDays(1); // extend this later
            });
        services.AddAuthorization(opts =>
        {
            opts.AddPolicy(ManagerOnly, policy => policy.RequireRole(nameof(UserRole.Manager)));
            opts.AddPolicy(AdminOnly, policy => policy.RequireRole(nameof(UserRole.Admin)));
            opts.AddPolicy(ManagerOrAdminOnly, policy => policy.RequireRole(nameof(UserRole.Manager), nameof(UserRole.Admin)));
        });

        services.AddCors();

#if DEBUG
        services.AddSwaggerGen(opts => opts.SwaggerDoc("v0", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Spreadsheet Components for Web-Based Projects API", Version = "v0" }));
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

        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors(b => b.WithOrigins(new List<string>(ProductionUrls).Append(DebugUrl).ToArray()).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

        app.UseAuthentication();
        app.UseAuthorization();

#if DEBUG
        app.UseSwagger();
        app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v0/swagger.json", "Spreadsheet Components for Web-Based Projects API"));
#endif

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGraphQL();
        });
    }
}
