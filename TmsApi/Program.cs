using Microsoft.AspNetCore.Authentication;
using TmsApi.Services;
using TmsApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddAuthentication("TrainingScheme")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "TrainingScheme",
        options => { });

builder.Services
    .AddOptions<EnrollmentOptions>()
    .Bind(builder.Configuration.GetSection("Enrollment"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
    
builder.Services.AddAuthorization();

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
