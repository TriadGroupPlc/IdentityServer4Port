using IdentityServer4PortApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Functions.BuildLogger();
builder.Logging.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseSwagger();

startup.Configure(app, app.Environment);

// endpoints
app.MapGet("/HelloWorld", () => "Hello, sup homie.");

app.MapGet("/HelloAuthorizedWorld", () => "Hello, sup authenticated homie.").RequireAuthorization();

app.UseSwaggerUI();

app.Run();