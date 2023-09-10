### IdentityServer4 port from .NET Core 3.1 to .NET 7 Minimal API format

### Concepts and technologies covered by this solution
- IdentityServer4
- .NET 7
- Minimal API
- Swagger
- Authentication

### What's in the repo
- **IdentityServer4**                         -       *Token Service*
- **IdentityServer4PortApi**                  -       *The API to be protected*
- **IdentityServer4PortConsoleClient**        -       *The client interacting with the API*

### Porting IdentityServer4

Obtain the final build of IdentityServer4 from [here](https://github.com/IdentityServer).

Clone locally and then navigate to `..\IdentityServer4\samples\Quickstarts\1_ClientCredentials\` folder and double-click the `Quickstart.sln` file to 
open the solution in Visual Studio (2022 used here).  

This solution is based on .NET Core 3.1 and will require porting to .NET 7 - we will do this by creating a .NET 7 project called *IdentityServer* in a new solution.

#### New project structure - IdentityServer

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

Add the following two packages to the **IdentityServer** project:

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

We have kept the structure of the Startup class and this is great for porting from existing .NET Core 3.1 projects which you can then later refactor and move the 
code into `Program.cs` or a custom class as you desire.

The above code registers the IdentityServer4 service set some basic options and then configure the service adding some resources such as In-Memory client, authentication etc.
*The code was kept in this structure to follow lean code concepts keeping the `Program.cs` free of code clutter.*

#### Wiring up in Program.cs
The `Startup.cs` class will be used in the `Program.cs` when configuring the Web Application Builder services:

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

The code above is fairly simply we have using statements (that can be [relocated](https://anthonygiretti.com/2021/07/18/introducing-c-10-global-usings-example-with-asp-net-core-6/) 
in a `GlobalUsings.cs` class...) one for [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) providing the Token Server functionality
and another to add some useful app logging provided by [Serilog](https://github.com/serilog/serilog-aspnetcore).

First we create a new instance of the Web Application Builder class for the Web Application which provides the [HTTP pipeline](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0).
Next we call a new class `Functions.cs` which builds the default SeriLog logger (and can do other work here) through a static method called `BuildLogger()`.

Once the services are configured for the middleware the Web Application is built and configured and finally executed (we have no endpoints so just needs to run as a web application listening on port 5000).

#### Functions.cs

Create a new class called `Functions.cs` and paste the below code into the new file:

```
    /// <summary>
    /// Functions used by the <see cref="Program"/> class
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Create a logger using the configured sinks
        /// </summary>
        public static void BuildLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }
    }
```

This class can be used to host static functions usable by the `Program.cs` class this aids in decluttering the main Program class in Minimal API.

#### Config.cs
Now we need to create the IdentityServer4 configuration class `Config.cs` that is used to define User Identity Scopes, API resources, access to scopes, clients and for the sake of this repository we are using the in-memory user object so we hardcode the test user(s) values here:

```
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources() => 
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiName")
                {
                    Scopes = new List<string>{ "Api.read", "Api.write" },
                    ApiSecrets = { new Secret("secret_for_the_api".Sha256()) }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("ApiName", "My API")
            };

        public static IEnumerable<Client> Clients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "ConsoleApp_ClientId",
                    ClientSecrets = { new Secret("secret_for_the_consoleapp".Sha256()) },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "Api.read" },
                }
            };

        public static List<TestUser> TestUsers() =>
            new()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "id4PortDemoUser",
                    Password = "W1nt3rL4k399", //.Sha265() removed because it was confusing withouth adding any value, https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "id4 Port Demo User),
                        new Claim(JwtClaimTypes.GivenName, "id4PortDemoUser"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.WebSite, "https://id4portdemouser.local/"),
                    }
                }
            };
    }
```

#### Launch settings
Modify `launchSettings.json` and replace with the following JSON:

```
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:5000",
      "sslPort": 0
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "SelfHost": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

### Running the project
Run the IdentityServer project to start up the web host console and launch your browser to the address https://localhost:5000.   Navigate to the following URL to confirm IdentityServer4 is running:

https://localhost:5000/.well-known/openid-configuration which should display a JSON response similar to the below example:

```
    "issuer": "http://localhost:5000",
    "jwks_uri": "http://localhost:5000/.well-known/openid-configuration/jwks",
    "authorization_endpoint": "http://localhost:5000/connect/authorize",
    "token_endpoint": "http://localhost:5000/connect/token",
    "userinfo_endpoint": "http://localhost:5000/connect/userinfo",
    "end_session_endpoint": "http://localhost:5000/connect/endsession",
    "check_session_iframe": "http://localhost:5000/connect/checksession",
    "revocation_endpoint": "http://localhost:5000/connect/revocation",
    "introspection_endpoint": "http://localhost:5000/connect/introspect",
    "device_authorization_endpoint": "http://localhost:5000/connect/deviceauthorization",
```

### GitHub repository

The [repository](https://github.com/tahir-khalid/IdentityServer4Port) provides the ported IdentityServer4 service running as a Token Server with an example API and clients to demonstrate username password login with token response using ResourceOwnerPassword grant (allows a client to send username &amp; password to the token service and get an access token back representing the user).