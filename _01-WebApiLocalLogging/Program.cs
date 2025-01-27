using Scalar.AspNetCore;
using WebApiLocalLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7156/scalar/v1
{
    options.WithTitle("Clever Content Website API");
});

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();

