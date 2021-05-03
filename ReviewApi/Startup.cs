using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using ReviewApi.AuthenticationHelper;

namespace ReviewRepo
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
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins(
                        "http://localhost:4200/", //for testing
                        "http://20.45.2.119/", //User
                        "http://20.189.29.112/", //Admintools
                        "http://20.94.137.143/" //Frontend
                    )
                );
            });
            var myConnectionString = Configuration.GetConnectionString("Cinephiliacs_Review");
            services.AddDbContext<Cinephiliacs_ReviewContext>(options =>
            {
                if (!options.IsConfigured)
                {
                    options.UseSqlServer(myConnectionString);
                }
            });
            services.AddScoped<IReviewLogic, ReviewLogic>();
            services.AddScoped<ReviewRepoLogic>();
            // for authentication
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "scheme";
            })
            .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(
                "scheme", o => { });

            var permissions = new[] {
                // "loggedin", // for signed in
                "manage:forums", // for moderator (is signed in)
                "manage:awebsite", // for admin (is moderator and signed in)
            };
            services.AddAuthorization(options =>
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    options.AddPolicy(permissions[i], policy => policy.RequireClaim(permissions[i], "true"));
                }
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReviewRepo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReviewRepo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
