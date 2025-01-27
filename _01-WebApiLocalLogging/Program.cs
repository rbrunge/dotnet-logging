using CommonLogging;
using Scalar.AspNetCore;
/*
   
   Default Logging Providers:
   
       Console: Logs are written to the console.
       Debug: Logs are written to the debug output.
       EventSource: Logs are written to an EventSource for monitoring and diagnostics.
       EventLog: On Windows, logs are written to the Windows Event Log1
       
       use command to show event source data:
       
         dotnet-trace collect --providers Microsoft-Windows-DotNETRuntime:4:4 --process-id 1234
   
 */



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7156/scalar/v1
{
    options.WithTitle(".WithTitle from Program.cs");
});

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();