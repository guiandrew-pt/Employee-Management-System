using DotNetEnv;
using EmployeeManagement.Repository.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Enable Cross-Origin Resource Sharing (CORS) for Blazor app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", builder =>
    {
        builder
            .WithOrigins("http://localhost:7274", "https://localhost:7274")  // Set the origin(port) to match the Blazor app’s address
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Allows cookies or authentication headers
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Enables endpoint discovery
builder.Services.AddSwaggerGen();           // Adds Swagger generation

// builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Register the repository
// Load environment variables in the API project
Env.TraversePath().Load();
string? connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured!");
}
// Register repository services via the extension method
builder.Services.AddRepositoryServices(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Adds Swagger JSON endpoint
    app.UseSwaggerUI();     // Adds Swagger UI
}

app.UseHttpsRedirection();

// Use the CORS policy
app.UseCors("AllowBlazorApp"); // Enable CORS using the defined policy for Blazor app

app.UseAuthorization();

app.MapControllers();

app.Run();

