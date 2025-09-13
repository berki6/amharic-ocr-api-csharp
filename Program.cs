using Serilog;
using Microsoft.OpenApi.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with more details
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Application", "AmharicOCRAPI")
	.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
	.WriteTo.File(
		path: "logs/ocr-api-.log",
		rollingInterval: RollingInterval.Day,
		outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
		retainedFileCountLimit: 7,
		fileSizeLimitBytes: 10_000_000,
		rollOnFileSizeLimit: true
	)
	.WriteTo.Seq(builder.Configuration["Seq:Url"] ?? "http://localhost:5341") // Centralized logging to Seq (configure URL in production)
	.CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();


// Register services for dependency injection
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<IOcrService, OcrService>();

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Set the 'Jwt:Key' configuration value.");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://yourfrontend.com") // Replace with actual allowed origins
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Swagger generation service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Amharic OCR API",
		Version = "v1",
		Description = "API to extract Amharic text from images and PDFs.",
		Contact = new OpenApiContact
		{
			Name = "Your Name or Team",
			Email = "your@email.com",
			Url = new Uri("https://yourwebsite.com")
		},
		// License = new OpenApiLicense
		// {
		// 	Name = "MIT License",
		// 	Url = new Uri("https://opensource.org/licenses/MIT")
		// }
	});

});




try
{
	Log.Information("Starting up Amharic OCR API...");
	var app = builder.Build();

	// Enable Swagger UI only in development
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "Amharic OCR API v1");
			c.DocumentTitle = "Amharic OCR API Documentation";
			c.RoutePrefix = "swagger";
			c.DefaultModelsExpandDepth(-1); // Hide schemas/models by default
			c.DisplayRequestDuration();
			c.EnableFilter();
			c.InjectStylesheet("/swagger-ui/custom.css"); // For custom CSS if needed
		});
	}

	app.UseHttpsRedirection();
	app.UseCors("AllowSpecificOrigins");
	app.UseAuthentication();
	app.UseMiddleware<ExceptionHandlingMiddleware>();
	app.UseAuthorization();
	app.MapControllers();

	app.Run();
	Log.Information("Amharic OCR API stopped cleanly.");
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed!");
	throw;
}
finally
{
	Log.CloseAndFlush();
}


