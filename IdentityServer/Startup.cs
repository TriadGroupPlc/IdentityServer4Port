namespace IdentityServer
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
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseSuccessEvents = true;
            }
                )
                .AddInMemoryIdentityResources(Config.IdentityResources())
                .AddInMemoryApiScopes(Config.ApiScopes())
                .AddInMemoryApiResources(Config.Apis())
                .AddInMemoryClients(Config.Clients())
                .AddJwtBearerClientAuthentication()
                .AddTestUsers(Config.TestUsers())
                .AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();
        }
    }
}
