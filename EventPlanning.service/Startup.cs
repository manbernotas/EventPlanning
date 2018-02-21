using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EventPlanning.BL;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventPlanning.service
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
            services.AddDbContext<DAL.EventContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            string smtpUser = Environment.GetEnvironmentVariable("smtpUser");
            string smtpPass = Environment.GetEnvironmentVariable("smtpPass");

            if (smtpUser == null || smtpPass == null)
            {
                throw new Exception("Smtp credentials not provided");
            }

            services.AddSingleton<ISmtpClient>(x => new SmtpWrapper("smtp.gmail.com", 587, smtpUser, smtpPass, true));

            var jwtIssuer = Environment.GetEnvironmentVariable("JWTIssuer");
            var jwtAudience = Environment.GetEnvironmentVariable("JWTAudience");
            var jwtKey = Environment.GetEnvironmentVariable("JWTKey");
            var jwtExp = Environment.GetEnvironmentVariable("JWTExpInMin");

            if (jwtIssuer == null || jwtAudience == null || jwtKey == null || jwtExp == null 
                || !Int32.TryParse(jwtExp, out var jwtExpAfter))
            {
                throw new Exception("JWT properties not provided");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(jwtExpAfter),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddMvc().AddJsonOptions(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
