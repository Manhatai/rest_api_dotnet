using MechanicsWorkshopApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var host = builder.Configuration["REST_API_DB_HOST"];
var port = builder.Configuration["REST_API_DB_PORT"];
var database = builder.Configuration["REST_API_DB_NAME"];
var username = builder.Configuration["REST_API_DB_LOGIN"];
var password = builder.Configuration["REST_API_DB_PASSWORD"];
var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

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
