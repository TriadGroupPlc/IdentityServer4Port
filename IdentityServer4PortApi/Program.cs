using IdentityModel.Client;
using IdentityServer4PortApi;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

Functions.BuildLogger();
builder.Logging.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
// setup the Swagger security options
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer4 Port API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseSwagger();

startup.Configure(app, app.Environment);

// endpoints
app.MapGet("/HelloWorld", () => "Hello, sup homie.");

app.MapGet("/HelloAuthorizedWorld", () => "Hello, sup authenticated homie.").RequireAuthorization();

app.MapGet("/LoginAsync", async () =>
{
    var httpClient = new HttpClient();

    //Make the call and get the result
    var identityServerResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
        Address = "http://localhost:5000/connect/token",

        // @Doc-Saintly, 2020-06-28: I think this line is unnecessary because RequestPasswordTokenAsync should automatically set the grant type
        // Issue: https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
        // GrantType = "password",

        ClientId = "83A0DD78191B4C3F8F932E27E95852E6",
        ClientSecret = "15F47BBA443B42D9B5AFA06648477F86",
        Scope = "IdentityServer4PortClientApi",

        UserName = "namjiAdmin",
        Password = "W1nt3rL4k399" //.Sha265() removed because it was confusing withouth adding any value, https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
    });

    //all good?
    if (!identityServerResponse.IsError)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("SUCCESS!!");
        Console.WriteLine();
        Console.WriteLine("Access Token: ");
        Console.WriteLine(identityServerResponse.AccessToken);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("Failed to login with error:");
        Console.WriteLine(identityServerResponse.ErrorDescription);
    }
});

app.UseSwaggerUI();

app.Run();