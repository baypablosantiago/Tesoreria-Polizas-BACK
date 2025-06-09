using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => //el temita de los cors
{
    options.AddPolicy("Warning-All", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TesoContext>(options
=> options.UseNpgsql(builder.Configuration.GetConnectionString("RenderPOSTGRES")));

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TesoContext>();

    // Ejecuta un TRUNCATE al iniciar la appweb, para poder hacer pruebas
    await context.Database.ExecuteSqlRawAsync(@"
        TRUNCATE TABLE ""endorsements"", ""policies"", ""policy_states"" RESTART IDENTITY CASCADE;
    ");
}

app.UseCors("Warning-All"); //el temita de los cors 2

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = string.Empty; // hace que Swagger esté disponible en "/"
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
