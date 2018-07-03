using System;
using System.IO;
using System.Linq;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using AngularSPAWebAPI.Services.Email;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace AngularSPAWebAPI
{
    public class Startup
    {
        private readonly IHostingEnvironment currentEnvironment;
    private readonly ILoggerFactory _loggerFactory;
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            currentEnvironment = env;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
      // SQLite & Identity.
      services.AddDbContext<ApplicationDbContext>(options =>
      {
        if (currentEnvironment.IsProduction())
        {
          options.UseMySql("Server=localhost ;Port=3306;Database=odsbe_;Uid=odsuser;Pwd = Catharina2018*; ");


        }
        else
        {
          options.UseMySql("Server=192.168.64.2 ;Port=3306;Database=ods;Uid=mo; ");

        }
      });

      

      services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
              // Lockout settings.
              options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
              options.Lockout.MaxFailedAccessAttempts = 20;
            });

            // Role based Authorization: policy based role checks.
            services.AddAuthorization(options =>
            {
                // Policy for dashboard: only administrator role.
                options.AddPolicy("Manage Accounts", policy => policy.RequireRole("administrator"));
                // Policy for resources: user or administrator roles. 
                options.AddPolicy("Access Resources", policy => policy.RequireRole("administrator", "user"));
            });

            // Adds application services.
            services.AddTransient<IDbInitializer, DbInitializer>();

            // Adds IdentityServer.
            services.AddIdentityServer()
                // The AddDeveloperSigningCredential extension creates temporary key material for signing tokens.
                // This might be useful to get started, but needs to be replaced by some persistent key material for production scenarios.
                // See http://docs.identityserver.io/en/release/topics/crypto.html#refcrypto for more information.
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                // To configure IdentityServer to use EntityFramework (EF) as the storage mechanism for configuration data (rather than using the in-memory implementations),
                // see https://identityserver4.readthedocs.io/en/release/quickstarts/8_entity_framework.html
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>(); // IdentityServer4.AspNetIdentity.


      if (currentEnvironment.IsProduction())
      {
        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
         .AddIdentityServerAuthentication(options =>
         {
           options.Authority = "http://jansenbyods.com";
           options.RequireHttpsMetadata = false;

           options.ApiName = "WebAPI";
         });


      }

      else
      {
        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                   .AddIdentityServerAuthentication(options =>
                   {
                     options.Authority = "http://localhost:5000";
                     options.RequireHttpsMetadata = false;
                     options.ApiName = "WebAPI";
                   });

       

      }

      var cors = new DefaultCorsPolicyService(_loggerFactory.CreateLogger<DefaultCorsPolicyService>())
      {
        AllowedOrigins = { "http://admin.jansenbyods.com", "http://jansenbyods.com", "http://localhost:4200", "http://localhost:4201"  }
      };
      services.AddSingleton<ICorsPolicyService>(cors);


      services.AddCors(options =>
      {
        options.AddPolicy("MyCorsPolicy",
   builder => builder
      .SetIsOriginAllowedToAllowWildcardSubdomains()
                          .WithOrigins("http://*.jansenbyods.com", "http://localhost:4200", "http://jansenbyods.com", "http://admin.jansenbyods.com", "http://localhost:4201")
      .AllowAnyMethod()
      .AllowCredentials()
      .AllowAnyHeader()
      .Build()
   );
      });



      services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Please insert JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

            });




        
      services.AddMvc();
     // services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
     // services.AddTransient<IEmailService, EmailService>();



    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();


      }
      else
      { app.UseDeveloperExceptionPage();      }


      app.UseCors("MyCorsPolicy");


      app.UseAuthentication();
      app.UseIdentityServer();




      // Microsoft.AspNetCore.StaticFiles: API for starting the application from wwwroot.
      // Uses default files as index.html.

      app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });



      app.UseCors("MyCorsPolicy");



      app.Use(async (context, next) => {
                await next();
                if (context.Response.StatusCode == 404 &&
                   !Path.HasExtension(context.Request.Path.Value) &&
                   !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            // Uses static file for the current path.
            app.UseStaticFiles();

      app.UseMvcWithDefaultRoute();
      app.UseMvc();

    }
  }
}
