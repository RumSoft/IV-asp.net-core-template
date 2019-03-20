using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using FluentValidation.AspNetCore;
using FluentValidation;
using AutoMapper;
using prezentacja_cis.Controllers;

namespace prezentacja_cis
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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging")); //log levels set in your configuration
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });

            app
                .UseMvc();

            Mapper.Initialize(cfg => {
                cfg.AddProfile<ChatAutoMapperProfile>();
            });

            //app
            //    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<MessageEntityValidator>());
        }
    }
}
