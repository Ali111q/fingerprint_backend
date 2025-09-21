using System.Text.Json;
using System.Text.Json.Serialization;
using FingerPrintVerfication.Extensions;
using FingerPrintVerfication.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    // x.JsonSerializerOptions.Converters.Add(new NullableFloatJsonConverter());
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,
        allowIntegerValues: true));
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*");
    });
    
    // Add a named policy for more explicit control
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*");
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS first, before HTTPS redirection
app.UseCors();

// Only redirect to HTTPS in production or when not in Docker
if (!app.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

// Configure static files serving for fingerprints
var fingerprintPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
    ? @"C:\fingerprints" 
    : "/app/fingerprints";

// Ensure the fingerprints directory exists
if (!Directory.Exists(fingerprintPath))
{
    Directory.CreateDirectory(fingerprintPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(fingerprintPath),
    RequestPath = "/fingerprints"
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
}

app.MapControllers();

app.Run();