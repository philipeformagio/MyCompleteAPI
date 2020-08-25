using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // AddApiVersioning - does the versioning by itself
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; //if you don't have specified version it will assumes the default version
                options.DefaultApiVersion = new ApiVersion(1, 0); // default api version
                options.ReportApiVersions = true; // while api is consumed, this option sends a message in the requet's header saying if the version is ok or obsolete
            });

            // AddVersionedApiExplorer - exposes the versioning helping the documentation to understand the API's versioning
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // versioning format
                options.SubstituteApiVersionInUrl = true; // substitute in the route(url) by a default api version
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // to use your own error custom response
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Development", builder =>
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());

                options.AddPolicy("Production", builder =>
                        builder
                        .WithMethods("GET")
                        .WithOrigins("http://desenvolvedor.io")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                        .AllowAnyHeader());

                //options.AddDefaultPolicy(
                //    buider =>
                //        buider
                //            .AllowAnyOrigin()
                //            .AllowAnyMethod()
                //            .AllowAnyHeader()
                //            .AllowCredentials());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfig(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseMvc();

            return app;
        }
    }
}
