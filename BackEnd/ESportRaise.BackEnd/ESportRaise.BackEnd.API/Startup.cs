using ESportRaise.BackEnd.API.Middleware;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.BLL.Services;
using ESportRaise.BackEnd.DAL.Interfaces;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace ESportRaise.BackEnd.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("confidential.json", false)
                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped(_ => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            var tokenFactoryService = new JwtTokenGeneratorService(Configuration);
            services.AddSingleton<IAuthTokenFactory>(tokenFactoryService);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // TODO: If dev, then disable
                    options.TokenValidationParameters = tokenFactoryService.TokenValidationParameters;
                });

            services.AddTransient<AppUserRepository>();
            services.AddTransient<TrainingAsyncRepository>();

            services.AddSingleton<IAuthTokenFactory, JwtTokenGeneratorService>();
            services.AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasherService>();

            services.AddTransient<IAuthAsyncService, AuthService>();
            services.AddTransient<IStreamingApiService, YouTubeV3Service>();
            services.AddTransient<ITrainingService, TrainingService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
