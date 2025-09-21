using System.Text.Json;
using System.Text.Json.Serialization;
using FingerPrintVerfication.Extensions;
using FingerPrintVerfication.Data;
using FingerPrintVerfication.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    // x.JsonSerializerOptions.Converters.Add(new NullableFloatJsonConverter());
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,
        allowIntegerValues: true));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder
            .WithOrigins("https://takeeldep.gov.iq", "https://www.takeeldep.gov.iq")
            .AllowAnyMethod()
            .AllowAnyHeader();
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

// Add custom CORS middleware FIRST - this will handle everything
app.UseMiddleware<CorsMiddleware>();

// Enable built-in CORS as backup

if (app.Environment.IsProduction())
{
    app.UseCors("Production");
}

app.UseCors("CorsPolicy");
;

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