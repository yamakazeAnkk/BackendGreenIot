using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenIotApi;
using GreenIotApi.Repositories;
using GreenIotApi.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using AutoMapper;
using GreenIotApi.Helpers;
using Google.Cloud.Firestore;
using GreenIotApi.Repositories.IRepositories;
using GreenIotApi.Services.InterfaceService;



var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton(provider =>
{
    // Set the Google Application Credentials environment variable
    string path = "bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json";
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

    // Create FirestoreDb instance
    return FirestoreDb.Create("bookstore-59884");
});

builder.Services.AddScoped<SensorDataRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddScoped<GardenService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<SensorDataService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddTransient<GardenRepository>();
builder.Services.AddTransient<DeviceRepository>();
builder.Services.AddTransient<SensorDataRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IGardenRepository), typeof(GardenRepository));
builder.Services.AddScoped(typeof(ISensorDataRepository), typeof(SensorDataRepository));
builder.Services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
builder.Services.AddScoped<FirebaseStorageService>();







FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json"),
});


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors("AllowAll");



if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenIotApi v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();