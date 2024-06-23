using MechanicsWorkshopApi.Data; // Imports data context from Data
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog; // For DB operations


var builder = WebApplication.CreateBuilder(args); // Used to configure and build the app, like below


// Lines below fetch .env variables which are used to create the database from appsettings.json
var host = builder.Configuration["REST_API_DB_HOST"];
var port = builder.Configuration["REST_API_DB_PORT"];
var database = builder.Configuration["REST_API_DB_NAME"];
var username = builder.Configuration["REST_API_DB_LOGIN"];
var password = builder.Configuration["REST_API_DB_PASSWORD"];
var secret_key = builder.Configuration["REST_API_JWT_SERCRET_KEY"];
var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";



builder.Services.AddControllers(); // Adds support for API endpoints 
builder.Services.AddEndpointsApiExplorer(); // 'Endpoints discovery' for tools like swagger
builder.Services.AddSwaggerGen(); // Registers swagger generation which is used to create Swagger docummentation
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString); // Registers the connectionString variable as the DB connection which uses
});                                      // previously retrieved config values. Dependency injection container - todo.



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret_key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


Log.Logger = new LoggerConfiguration() // Serilog docs for more
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/logs-.txt", rollingInterval: RollingInterval.Day) // new logs each day :D
    .CreateLogger();

var app = builder.Build(); // 'app = Flask(__name__)' essentially.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Enables the API docummentation testing for swagger if ran in Development mode
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection(); // Middleware for redirecting from HTTP to HTTPS
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run(); // Runs the app


/*
Todo:
- Logging
- Clean architecture
 */