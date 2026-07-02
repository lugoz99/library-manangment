using FluentResults.Extensions.AspNetCore;
using LibraryManagment.Data;
using LibraryManagment.Helpers;
using LibraryManagment.Repository;
using LibraryManagment.Repository.Interfaces;
using LibraryManagment.Repository.IRepository;
using LibraryManagment.Services.Contracts;
using LibraryManagment.Services.Implementation;
using Mapster;
using MapsterMapper;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MAPSTER (Object Mapping)
// =====================================================

var config = TypeAdapterConfig.GlobalSettings;

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddMapster();

// =====================================================
// FLUENTRESULTS (Error Handling)
// =====================================================

// Allows access to the current HttpContext
builder.Services.AddHttpContextAccessor();

// Register custom profile that converts FluentResults
// errors into HTTP responses (400, 404, 409, etc.)
builder.Services.AddSingleton<FluentResultsEndpointProfile>();

// =====================================================
// DATABASE
// =====================================================

var connectionString = builder.Configuration
    .GetConnectionString("ConexionSql");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// =====================================================
// API SERVICES
// =====================================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();


//==========================================================
// DEPENDENCY INJECTION REPOSITORY AND SERVICES
//===========================================================
builder.Services.AddScoped<IUnitWork, UnitWork>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();

var app = builder.Build();

// =====================================================
// FLUENTRESULTS CONFIGURATION
// =====================================================

// Get required services from the DI container
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
var profile = app.Services.GetRequiredService<FluentResultsEndpointProfile>();

// Give the profile access to the current HttpContext
profile.SetHttpContextProvider(() => httpContextAccessor.HttpContext!);

// Configure FluentResults to use our custom profile
// when converting Results to HTTP responses
AspNetCoreResult.Setup(options =>
{
    options.DefaultProfile = profile;
});

// =====================================================
// HTTP PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "library API";
        options.Theme = ScalarTheme.DeepSpace;
        options.Layout = ScalarLayout.Modern;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();