using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TmsApi.Services;
using TmsApi.Configuration;
using TmsApi.Infrastructure;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Filters;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddDbContext<TmsDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging();
});

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

builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddProblemDetails();

// Ensure using TmsApi.Filters; is at the top
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseCors("AllowAngular");

app.UseExceptionHandler();

// Configure the HTTP request pipeline.

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed test data at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    if (!context.Students.Any())
    {
        var students = new List<Student>
        {
            new()
            {
                RegistrationNumber = "TMS-2026-0001",
                Name = "Ali Shemsu",
                GPA = 3.8m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0002",
                Name = "Bonny Jemal",
                GPA = 2.9m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0003",
                Name = "Carisma Birhanu",
                GPA = 3.4m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0004",
                Name = "Dina Petros",
                GPA = 3.9m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0005",
                Name = "Eden Walelgn",
                GPA = 2.5m,
                IsActive = true
            }
        };

        context.Students.AddRange(students);

        var courses = new List<Course>
        {
            new()
            {
                Code = "CS-101",
                Title = "Introduction to Computer Science",
                MaxCapacity = 30
            },
            new()
            {
                Code = "CS-201",
                Title = "Data Structures and Algorithms",
                MaxCapacity = 25
            },
            new()
            {
                Code = "MAT-101",
                Title = "Calculus I",
                MaxCapacity = 40
            }
        };

        context.Courses.AddRange(courses);

        context.SaveChanges();

        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[0].Id,
                Grade = 4.0m
            },
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[1].Id,
                Grade = 3.6m
            },
            new()
            {
                StudentId = students[1].Id,
                CourseId = courses[0].Id,
                Grade = 2.8m
            },
            new()
            {
                StudentId = students[3].Id,
                CourseId = courses[1].Id,
                Grade = 3.9m
            }
        };

        context.Enrollments.AddRange(enrollments);

        context.SaveChanges();
    }
}

app.Run();
