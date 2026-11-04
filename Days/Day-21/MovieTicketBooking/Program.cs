using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Models;
using MovieTicketBooking.Authorization;
using MovieTicketBooking.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CORS CONFIGURATION
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// 2. DATABASE CONFIGURATION
// ==========================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// 3. IDENTITY CONFIGURATION
// ==========================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ==========================================
// 4. JWT AUTHENTICATION
// ==========================================

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ==========================================
// 5. AUTHORIZATION HANDLER
// ==========================================

builder.Services.AddScoped<
    IAuthorizationHandler,
    BookingAuthorizationHandler>();

// ==========================================
// 6. REPOSITORIES
// ==========================================

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ==========================================
// 7. SERVICES
// ==========================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IShowSeatService, ShowSeatService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// ==========================================
// 8. CONTROLLERS AND OPENAPI
// ==========================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// 9. DATABASE SEEDING
// ==========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await DbSeeder.SeedRolesAsync(roleManager);

    await DbSeeder.SeedAdminAsync(
        userManager,
        app.Configuration);
}

// ==========================================
// 10. EXCEPTION HANDLING
// ==========================================

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature =
            context.Features.Get<IExceptionHandlerFeature>();

        var exception = exceptionFeature?.Error;

        context.Response.ContentType = "application/json";

        if (exception is ArgumentException)
        {
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = exception.Message
            });
        }
        else
        {
            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "An unexpected server error occurred."
            });
        }
    });
});

// ==========================================
// 11. HTTP REQUEST PIPELINE
// ==========================================

// HTTPS Redirection
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// CORS
app.UseCors("AngularFrontend");

// Endpoint Metadata Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<EndpointMetadataMiddleware>();
}

// Authentication
app.UseAuthentication();

// Authentication Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDiagnosticsMiddleware>();

    // Authorization Diagnostics
    app.UseMiddleware<AuthorizationDiagnosticsMiddleware>();
}

// Authorization
app.UseAuthorization();

// ==========================================
// 12. MAP CONTROLLERS
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

// ==========================================
// 13. RUN APPLICATION
// ==========================================

app.Run();
