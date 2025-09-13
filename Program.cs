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

	// Enable file upload support in Swagger UI
	c.OperationFilter<FileUploadOperationFilter>();
});

// Add support for file upload in Swagger UI
builder.Services.AddSwaggerGenNewtonsoftSupport();

// File upload operation filter
public class FileUploadOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
	public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
	{
		var fileParams = context.MethodInfo.GetParameters()
			.Where(p => p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile));
		if (fileParams.Any())
		{
			operation.RequestBody = new OpenApiRequestBody
			{
				Content =
				{
					["multipart/form-data"] = new OpenApiMediaType
					{
						Schema = new OpenApiSchema
						{
							Type = "object",
							Properties =
							{
								[fileParams.First().Name] = new OpenApiSchema
								{
									Type = "string",
									Format = "binary"
								}
							},
							Required = new HashSet<string> { fileParams.First().Name }
						}
					}
				}
			};
		}
	}
}

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
