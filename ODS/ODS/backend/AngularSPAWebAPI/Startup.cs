using System;
using System.IO;
using System.Linq;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace AngularSPAWebAPI
{
    public class Startup
    {
    private readonly IHostingEnvironment currentEnvironment;
    private readonly ILoggerFactory _loggerFactory;
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            currentEnvironment = env;
            _loggerFactory = loggerFactory;
        }


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
          options.UseMySql("Server=server10.exacthost.nl ;Port=3306;Database=odsbe_;Uid=odsuser;Pwd = Catharina2018*;  ");
       //   options.UseMySql("Server=localhost ;Port=3306;Database=mo;Uid=root;Pwd =Boeras23; ");

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
            
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Manage Accounts", policy => policy.RequireRole("administrator"));
                options.AddPolicy("Access Resources", policy => policy.RequireRole("administrator", "user"));
            });

            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();


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
                     options.Authority = "http://localhost:55646";
                     options.RequireHttpsMetadata = false;
                     options.ApiName = "WebAPI";
                   });

       

      }

      var cors = new DefaultCorsPolicyService(_loggerFactory.CreateLogger<DefaultCorsPolicyService>())
      {
        AllowedOrigins = { "http://admin.jansenbyods.com", "https://admin.jansenbyods.com'", "http://localhost:4200", "http://jansenbyods.com", "https://jansenbyods.com", "http://localhost:4201", "http://localhost:55646" }
      };
      services.AddSingleton<ICorsPolicyService>(cors);

      services.AddSingleton<IFileProvider>(
               new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


      services.AddCors(options =>
      {
        options.AddPolicy("MyCorsPolicy",
   builder => builder
      .SetIsOriginAllowedToAllowWildcardSubdomains()
      .WithOrigins("http://*.jansenbyods.com", "https://admin.jansenbyods.com" ,"http://admin.jansenbyods.com'", "http://localhost:4200", "http://jansenbyods.com", "http://admin.jansenbyods.com", "http://localhost:4201", "http://localhost:55646")
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
      services.AddSingleton<IEmailConfiguration>(new EmailConfiguration());
      services.AddTransient<IEmailService, EmailService>();




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


      app.Use((context, next) =>
      {
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        return next.Invoke();
      });

      app.UseDefaultFiles();
      app.UseStaticFiles();
    

      app.UseMvcWithDefaultRoute();
      app.UseMvc();

    }
  }
}
