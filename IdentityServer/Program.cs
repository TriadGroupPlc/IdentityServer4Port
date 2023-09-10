using IdentityServer;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

Functions.BuildLogger();
builder.Logging.AddSerilog();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

// endpoints

app.Run();