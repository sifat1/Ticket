using App.Services;
using DB.DBcontext;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Ticket.EventHandler;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

builder.Services.AddScoped<ShowService>();
builder.Services.AddScoped<VenueService>();
builder.Services.AddScoped<StandService>();
builder.Services.AddScoped<EventPublisher>();
builder.Services.AddScoped<ShowAddedHandler>();
builder.Services.AddScoped<StandAddedHandler>();

builder.Services.AddHangfireConfig(builder.Configuration);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
