using CommonLogging;
using Scalar.AspNetCore;
/*
   
   Default Logging Providers:
   
       Console: Logs are written to the console.
       Debug: Logs are written to the debug output.
       EventSource: Logs are written to an EventSource for monitoring and diagnostics.
       EventLog: On Windows, logs are written to the Windows Event Log1
   
 */



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7156/scalar/v1
{
    options.WithTitle("01 from Program.cs");
});

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();