using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Services;

namespace TitlesWebGame.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TitlesWebGame.Api", Version = "v1"});
            });

            services.AddSingleton<IGameSessionManager, GameSessionManager>();
            
            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebClient", builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins("https://localhost:8001")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TitlesWebGame.Api v1"));
            }

            app.UseCors("AllowWebClient");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TitlesGameHub>("/game");
                endpoints.MapControllers();
            });
        }
    }
}