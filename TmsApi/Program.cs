using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddAuthentication("TrainingScheme")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "TrainingScheme",
        options => { });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
