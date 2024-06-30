using MechanicsWorkshopApi.Helpers;
using MechanicsWorkshopApi.Infra.Data;
using MechanicsWorkshopApi.Security;
using Microsoft.EntityFrameworkCore;
using Serilog;



var builder = WebApplication.CreateBuilder(args); // Used to configure and build the app, like below


// Lines below fetch .env variables which are used to create the database from appsettings.json
var host = builder.Configuration["REST_API_DB_HOST"];
var port = builder.Configuration["REST_API_DB_PORT"];
var database = builder.Configuration["REST_API_DB_NAME"];
var username = builder.Configuration["REST_API_DB_LOGIN"];
var password = builder.Configuration["REST_API_DB_PASSWORD"];
var secret_key = builder.Configuration["REST_API_JWT_SK"];
var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";


builder.Services.AddControllers(); // Adds support for API endpoints 
builder.Services.AddEndpointsApiExplorer(); // 'Endpoints discovery' for tools like swagger
builder.Services.AddSwaggerGen(); // Registers swagger generation which is used to create Swagger docummentation
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString); // Registers the connectionString variable as the DB connection which uses
});                                      // previously retrieved env values. Dependency injection container - todo.



builder.Services.AddJwtAuthentication(builder.Configuration);


Log.Logger = new LoggerConfiguration() // Serilog docs for more
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/logs-.txt", rollingInterval: RollingInterval.Day) // New logs each day
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
app.Use(async (context, next) => await ErrorHandlingHelper.HandleGlobalErrors(context, () => next(context))); // Global error handler
app.Run(); // Runs the app


/*
Todo:
- Clean architecture
 */