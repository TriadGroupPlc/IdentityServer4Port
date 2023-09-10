### IdentityServer4 port from .NET Core 3.1 to .NET 7 Minimal API format

### Concepts and technologies covered by this solution
- IdentityServer4
- .NET 7
- Minimal API
- Swagger
- Authentication

### Porting IdentityServer4

Obtain the final build of IdentityServer4 from [here](https://github.com/IdentityServer).

Clone locally and then navigate to `..\IdentityServer4\samples\Quickstarts\1_ClientCredentials\` folder and double-click the `Quickstart.sln` file to 
open the solution in Visual Studio (2022 used here).  

This solution is based on .NET Core 3.1 and will require porting to .NET 7 - we will do this by creating a .NET 7 project called *IdentityServer* in a new solution.

#### New project structure

There are some differences now  between .NET Core 3.1 and newer dotnet projects and we focus on Minimal API in this guide:

- Config.cs			-	IdentityServer4 configuration class where we define clients, scopes etc
- Functions.cs		-	A helper class to reduce code footprint in the `Program.cs` class
- Program.cs		-	Single program class with top level statements
- Startup.cs		-	Mimics previous startup class allows for seperation of concern and keeping `Program.cs` with a low code footprint

#### Program.cs 

Empty the contents of `Program.cs` class and copy-paste the following code:

```
			using IdentityServer;
			using Serilog;

			var builder = WebApplication.CreateBuilder(args);

			Functions.BuildLogger();
			builder.Logging.AddSerilog();

			var startup = new Startup(builder.Configuration);
			startup.ConfigureServices(builder.Services);

			var app = builder.Build();

			startup.Configure(app, app.Environment);

			// endpoints

			app.Run();
```

Add the following two packages to the project:

- IdentityServer4
- Serilog.AspNetCore

#### Startup.cs

Create a new class called `Startup.cs` in the root of the project and copy-paste the following code:

```
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
```

