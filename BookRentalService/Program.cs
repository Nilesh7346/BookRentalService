using API.Filter;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Helpers;
using Services.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateUserBookIdsAttribute>(); // Apply globally
});
builder.Services.AddScoped<ActivityLogger>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IBookRentalService, BookRentalService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHostedService<OverdueRentalService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<PerformanceLogger>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
