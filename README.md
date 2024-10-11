

# spahost

The goal is hosting a SPA (react) from ASP.NET without using the Visual Studio template or using the ASP.NET SPA services. The same strategy can be used for any other SPA framework.

This completely separates the front-end and the back-end tasks which are normally developed by separate teams. Also, it's more likely the UI to be developed using Visual Studio Code while the back-end with Visual Studio.

The folder structure is supposed to be the following:

```
<some folder>\spahost
├───HostingApp
│   └───HostingApp
│       ├───Controllers
│       ├───Properties
│       └───wwwroot       (this will be entirely wiped and overwritten)
└───spa
    └───frontend
        └───package.json
```

* `\spahost\HostingApp` contains  the Visual Studio solution file
* `\spahost\HostingApp\HostingApp` contains  the ASP.NET project
* `\spahost\HostingApp\HostingApp\wwwroot` is the folder that will be wiped and overwritten with the artifacts produced by `npm run build`
* `\spahost\spa\frontend` contains the react project created with `npx create-react-app frontend`. The `package.json` file is in this folder.

## The react application

The application is created using:

```
spahost\spa>npx create-react-app frontend
```

Create the `.env` file in the root folder (`frontend`) with the following content:

```
BUILD_PATH=../../HostingApp/HostingApp/wwwroot
```

Build the react application using:

```
npm run build
```

The build now creates the `wwwroot` folder in the `HostingApp` project.

## ASP.NET project

Create the `HostingApp` ASP.NET Web API project. The `Program.cs` file requires few changes. The following is just the relevant portion of the configuration:

```
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
app.UseCors(corsPolicy);    // <==== add this
app.UseRouting();           // <==== add this
app.UseStaticFiles();       // <==== add this


app.UseAuthorization();


//app.MapControllers();          // <==== remove this
// add the following:
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("/index.html");
});

app.Run();
```

* `AddCors` and `UseCors` are required during the frontend development but not from the deployed application. During development the `frontend` app is hosted on a different domain (the development server) and requires the WebApi to know it. After publishing, everything is served from the Kestrel webserver and Cors is not needed anymore.
* `UseRouting` is needed to configure the routing with `UseEndopoints`
* `UseStaticFiles` by default looks into the `wwwroot`
* `MapControllers` must be replaced with `UseEndpoints`. Starting from .NET 8 the analyzer suggest migrating the `UseEndopoints` to `app.MapFallbackToFile("/index.html");`

## Minimal API

The project `HostingAppMinimalAPI` is the exact replica of `HostingAppMinimal` but uses the latest (.NET 8) template based on the Minimal API in ASP.NET Core which is the preferred method in ASP.NET Core to serve the endpoints.

There is no much difference, but I preferred to publish a separate project for this new template to avoid any kind of confusion.

## Final considerations

Still looking for a smart way to integrate the SPA build process into VS compilation. I definitely don't want to tie this to the VS publish or compile.

Currently, to update the `wwwroot`, just run `npm run build` from the frontend app.

