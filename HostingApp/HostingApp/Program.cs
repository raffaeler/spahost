namespace HostingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var corsPolicy = "HostingAppCorsPolicy";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Cors configuration is needed during front-end development
            // because react is hosted in a different domain 
            // which is typically localhost:3000
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, policy =>
                {
                    policy
                        //.AllowAnyOrigin()
                        .WithOrigins("https://localhost:3000", "http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
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


            // The template generates:
            //app.MapControllers();

            // The following code was used in .NET 6
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.MapFallbackToFile("/index.html");
            //});

            // This other code uses the minimal API:
            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}