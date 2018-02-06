using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EventPlanning.BL;
using System;

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

            services.AddMvc().AddJsonOptions(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
