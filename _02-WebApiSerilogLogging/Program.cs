using CommonLogging;
using Scalar.AspNetCore;
using Serilog;
using Log = Serilog.Log;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
app.UseSerilogRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7727/scalar/v1
{
    options.WithTitle("02 from Program.cs");
});

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();

