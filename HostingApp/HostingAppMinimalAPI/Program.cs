
namespace HostingAppMinimalAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var corsPolicy = "HostingAppCorsPolicy";
        var builder = WebApplication.CreateBuilder(args);

        // Cors configuration is needed during front-end development
        // because react is hosted in a different domain 
        // which is typically localhost:3000
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy, policy =>
            {
                policy
                    //.AllowAnyOrigin()
                    .WithOrigins(
                        "https://localhost:3000",
                        "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(corsPolicy);    // raf
        app.UseRouting();           // raf
        app.UseStaticFiles(new StaticFileOptions()
        {
            // set this to true to serve WebAssembly files
            // set this to false to serve other web-only SPA apps like react
            //
            ServeUnknownFileTypes = true
        }); // raf

        app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
