using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => //el temita de los cors
{
    options.AddPolicy("AllowLocalhost4200", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TesoContext>(options 
=> options.UseMySQL(builder.Configuration.GetConnectionString(name:"XAMPP")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // para que funcione swagger

builder.Services.AddControllers(); // añadir los controladores

builder.Services.AddScoped<EmailRetriverService>();
builder.Services.AddScoped<EmailScannerService>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    TesoContext context = scope.ServiceProvider.GetRequiredService<TesoContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowLocalhost4200"); //cors temario
// Configurar Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = string.Empty; // Hace que Swagger esté disponible en "/"
    });
}

app.UseHttpsRedirection();

app.MapControllers(); // Mapea? los controladores

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
