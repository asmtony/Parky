using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ParkyApi.Data;
using ParkyApi.OptionsHelper;
using ParkyApi.ParkyMapper;
using ParkyApi.Repository;
using ParkyApi.Repository.IRepository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkyApi
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
            services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen();
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("ParkyOpenApiSpec", new Microsoft.OpenApi.Models.OpenApiInfo()
            //    {
            //        Title = "Parky Api",
            //        Version = "1",
            //        Description = "Udemy Parky Api",
            //        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //        {
            //            Email = "me@there.com",
            //            Name = "ASM",
            //            Url = new Uri("https://convert2pdfpro.com/")
            //        },
            //        License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //        {
            //            Name = "MIT Licence",
            //            Url = new Uri("https://en.wikipedia.org/wiki/MIT_Licence")
            //        }
            //    });
            ////    options.SwaggerDoc("ParkyOpenApiSpecTrail", new Microsoft.OpenApi.Models.OpenApiInfo()
            ////    {
            ////        Title = "Parky Api (Trail)",
            ////        Version = "1",
            ////        Description = "Udemy Parky Api Trail",
            ////        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            ////        {
            ////            Email = "me@there.com",
            ////            Name = "ASM",
            ////            Url = new Uri("https://convert2pdfpro.com/")
            ////        },
            ////        License = new Microsoft.OpenApi.Models.OpenApiLicense()
            ////        {
            ////            Name = "MIT Licence",
            ////            Url = new Uri("https://en.wikipedia.org/wiki/MIT_Licence")
            ////        }
            ////    });
            //    var xmlCommentsFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            //    options.IncludeXmlComments(cmlCommentsFullPath);
            //});
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var desc in provider.ApiVersionDescriptions)               
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", 
                        desc.GroupName.ToUpperInvariant());
               
                options.RoutePrefix = "";
            });

            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json", "Parky API");
            //    //options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecTrail/swagger.json", "Parky API Trail");
            //});

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
