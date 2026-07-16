using FluentResults.Extensions.AspNetCore;
using LibraryManagement.Data;
using LibraryManagement.Helpers;
using LibraryManagement.Repository;
using LibraryManagement.Repository.Interfaces;
using LibraryManagement.Services.Contracts;
using LibraryManagement.Services.Implementation;
using Mapster;
using MapsterMapper;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

// 1. Load environment variables from the .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. CRITICAL: Tell .NET to include system and .env environment variables in the configuration provider
builder.Configuration.AddEnvironmentVariables();

// =====================================================
// MAPSTER (Object Mapping)
// =====================================================
// Mapster copies data between data entities and DTOs automatically.
var config = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddMapster();

// =====================================================
// FLUENT RESULTS (Error Handling)
// =====================================================
// This allows services to access the current HTTP request information.
builder.Services.AddHttpContextAccessor();

// Register our custom error profile. It automatically changes FluentResults 
// errors (like NotFound) into correct HTTP codes (like 404).
builder.Services.AddSingleton<FluentResultsEndpointProfile>();

// =====================================================
// DATABASE CONFIGURATION
// =====================================================
// 3. FIX: Access your custom '.env' variable directly. If it is null, fall back to appsettings.
var connectionString = Environment.GetEnvironmentVariable("CONNECTION__STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions => 
        sqlServerOptions.EnableRetryOnFailure()));

// =====================================================
// API AND DOCUMENTATION SERVICES
// =====================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to generate the OpenAPI specification file (swagger.json).
// Scalar will read this file to display the modern user interface.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Library Management API",
        Version = "v1",
        Description = "API for managing categories and subcategories in a library system."
    });

    // Read the XML file to show your code comments inside the API documentation.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var path = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(path);
});

// Register repositories and services in the Dependency Injection container.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();

var app = builder.Build();

// =====================================================
// FLUENT-RESULTS CONFIGURATION
// =====================================================
// Get the required services from the system after the application is built.
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
var profile = app.Services.GetRequiredService<FluentResultsEndpointProfile>();

// Give the error profile access to the current HTTP context.
profile.SetHttpContextProvider(() => httpContextAccessor.HttpContext!);

// Configure FluentResults to use our profile for all HTTP responses.
AspNetCoreResult.Setup(options =>
{
    options.DefaultProfile = profile;
});

// =====================================================
// HTTP REQUEST PIPELINE (Middleware)
// =====================================================
// Enable documentation tools only when developing the application.
if (app.Environment.IsDevelopment())
{
    // Generates the swagger.json file at /swagger/v1/swagger.json
    app.UseSwagger();

    // Configure the Scalar user interface using the Swagger JSON file.
    app.MapScalarApiReference(options =>
    {
        options.Title = "Library API";
        options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
        options.Theme = ScalarTheme.DeepSpace;
        options.Layout = ScalarLayout.Modern;
    });
}

// Print the correct Scalar URL in the console when the application starts.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var url = app.Urls.FirstOrDefault(u => u.StartsWith("https")) ?? app.Urls.FirstOrDefault() ?? "http://localhost:5063";
    Console.WriteLine($"👉 Scalar UI: {url}/scalar");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
