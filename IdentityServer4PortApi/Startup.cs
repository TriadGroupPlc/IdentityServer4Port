using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.AccessTokenValidation;

namespace IdentityServer4PortApi
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public IConfiguration Config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5000";
                        options.RequireHttpsMetadata = false;

                        options.ApiName = "IdentityServer4PortClientApi";
                        options.ApiSecret = "D46CB03C51C74D998B331066816B133F";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthorization();

            //app.UseRouting();

            //app.UseHttpsRedirection();

            //app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
