using CommonLogging;
using Scalar.AspNetCore;
using Serilog;
using Log = Serilog.Log;

var builder = WebApplication.CreateBuilder(args);

// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Debug()
//     .WriteTo.Console()
//     .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
//
// var msg = "Hello, World! " + DateTime.Now.ToString("u");
// Log.Information(msg);

builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
app.UseSerilogRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7277/scalar/v1
{
    options.WithTitle("02 from Program.cs");
});


app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();

