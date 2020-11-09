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
using Swashbuckle.AspNetCore.Swagger;
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
                .AddJsonFile("confidential.json", false, true)
                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddMvc() 
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient(_ => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            var tokenFactoryService = new JwtTokenGeneratorService(Configuration);
            services.AddSingleton<IAuthTokenFactory>(tokenFactoryService);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenFactoryService.TokenValidationParameters;
                });

            services.AddTransient<AppUserRepository>();
            services.AddTransient<CriticalMomentRepository>();
            services.AddTransient<StateRecordRepository>();
            services.AddTransient<TeamMemberRepository>();
            services.AddTransient<TeamRepository>();
            services.AddTransient<TrainingRepository>();
            services.AddTransient<VideoStreamRepository>();

            services.AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasherService>();

            services.AddTransient<IAppUserService, AppUserService>();
            services.AddTransient<IAuthAsyncService, AuthService>();
            services.AddTransient<IConfigChangeService, ConfigChangeService>();
            services.AddTransient<ICriticalMomentService, CriticalMomentService>();
            services.AddTransient<IStateRecordService, StateRecordService>();
            services.AddTransient<IStressRecognitionService, StressRecognitionService>();
            services.AddTransient<ITeamMemberService, TeamMemberService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<ITrainingService, TrainingService>();
            services.AddTransient<IYouTubeService, YouTubeV3Service>();

            services.AddTransient<DatabaseRepository>();
            services.AddTransient<IDatabaseBackupService, DatabaseBackupService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ESR API");
                });

            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseMvc();
        }
    }
}
