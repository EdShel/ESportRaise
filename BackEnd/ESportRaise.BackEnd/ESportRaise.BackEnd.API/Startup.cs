﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.BLL.Services;
using ESportRaise.BackEnd.DAL.Interfaces;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ESportRaise.BackEnd.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddSingleton<IAuthTokenFactory, JwtTokenGeneratorService>();
            services.AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasherService>();
            services.AddTransient<UserAsyncRepository>();

            services.AddTransient<IAuthAsyncService, AuthService>();
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

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
