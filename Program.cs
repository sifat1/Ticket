using System.Text;
using App.Services;
using App.Services.Manager;
using DB.DBcontext;
using Hangfire;
using JWTAuthServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ticket.EventHandler;
using User.Registration;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000")  // Allow any origin
               .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader(); // Allow any headers
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

builder.Services.AddScoped<ShowService>();
builder.Services.AddScoped<VenueService>();
builder.Services.AddScoped<StandService>();
builder.Services.AddScoped<ShowManagerService>();
builder.Services.AddScoped<UserRegistrationService>();
builder.Services.AddScoped<EventPublisher>();
builder.Services.AddScoped<ShowAddedHandler>();
builder.Services.AddScoped<StandAddedHandler>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddHangfireConfig(builder.Configuration);

//  Move authentication **before** builder.Build()
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddHttpContextAccessor();

//  Now build the app
var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthentication(); //  Authentication middleware must come **before** authorization
app.UseAuthorization();

app.MapControllers();
app.Run();
