using Microsoft.OpenApi.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Register services for dependency injection
builder.Services.AddControllers();
builder.Services.AddScoped<IOcrService, OcrService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
