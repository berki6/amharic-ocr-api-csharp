# Amharic OCR API (C#)

A robust RESTful API for extracting Amharic text from images and PDF files using IronOCR, b## Logging

- Logs are written to the console and to `logs/ocr-api-<date>.log`.
- Log level: Debug and above.
- Includes request events, warnings, and errors.
- **Centralized logging:** Logs are also sent to Seq ([http://localhost:5341](http://localhost:5341) by default) for monitoring and troubleshooting. In production, configure the Seq URL in `appsettings.json` or environment variables.

## Monitoring

- **Application Insights:** Integrated for monitoring performance, failures, and usage metrics.
- Configure the connection string in `appsettings.json` for Azure Application Insights.
- Set up alerts for high error rates, slow responses, or other issues.ith ASP.NET Core.

## Tags

![Build Status](https://github.com/berki6/amharic-ocr-api-csharp/actions/workflows/dotnet.yml/badge.svg)
![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![C#](https://img.shields.io/badge/language-C%23-239120)
![IronOCR](https://img.shields.io/badge/IronOCR-OCR-green)
![Serilog](https://img.shields.io/badge/logging-Serilog-blue)
![Swagger](https://img.shields.io/badge/docs-Swagger-yellow)
![Amharic](https://img.shields.io/badge/language-Amharic-orange)

**Topics:** `OCR`, `Amharic`, `C#`, `ASP.NET Core`, `IronOCR`, `Serilog`, `Swagger`, `REST API`, `PDF`, `Image Processing`

## Features

- Extracts text from images (JPG, PNG, GIF, TIFF, BMP) and PDFs
- Preprocessing for improved OCR accuracy (deskew, denoise, enhance resolution)
- Structured logging with Serilog (console, file, and centralized with Seq)
- Monitoring and alerting with Application Insights
- Authentication and authorization with JWT Bearer tokens
- File upload validation (type, size)
- HTTPS enforcement and CORS policies for security
- Consistent, structured API responses
- Swagger UI for interactive API documentation and testing
- Secure configuration (no hardcoded secrets)

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [IronOCR License Key](https://ironsoftware.com/csharp/ocr/licensing/) (trial or production)

### Setup

1. **Clone the repository:**

   ```sh
   git clone https://github.com/berki6/amharic-ocr-api-csharp.git
   cd amharic-ocr-api-csharp
   ```

2. **Restore dependencies:**

   ```sh
   dotnet restore
   ```

3. **Configure environment variables:**
   - Copy `appsettings.template.json` to `appsettings.json` and fill in your values, or set environment variables:
     - `IronOcr__LicenseKey`: Your IronOCR license key
     - `Seq__Url`: Seq server URL (e.g., `http://localhost:5341` for development)
     - `ApplicationInsights__ConnectionString`: Azure Application Insights connection string
     - `Jwt__Key`: A strong secret key for JWT signing (at least 256 bits)
   - For development, you can use User Secrets: `dotnet user-secrets set "IronOcr:LicenseKey" "YOUR_KEY"`
   - In production, set these as environment variables in your deployment platform (Azure App Service, Docker, etc.)

4. **Build the project:**

   ```sh
   dotnet build
   ```

5. **Run the API (Development mode):**

   ```sh
   $env:ASPNETCORE_ENVIRONMENT="Development"
   dotnet run
   ```

## API Usage

### Endpoint: `POST /api/ocr/extract`

- **Description:** Upload an image or PDF file to extract Amharic text.
- **Content-Type:** `multipart/form-data`
- **Form field:** `file` (image or PDF)

#### Example with httpie

```sh
http -f POST http://localhost:5000/api/ocr/extract file@/path/to/image-or-pdf
```

#### Example with curl

```sh
curl -X POST http://localhost:5000/api/ocr/extract -F "file=@/path/to/image-or-pdf"
```

#### Example Response

- **Success Response:**

```json
{
  "success": true,
  "error": "",
  "data": {
    "outputFile": "C:/.../output/yourfile.txt",
    "text": "...extracted text..."
  },
  "request": {
    "fileName": "yourfile.png",
    "fileSize": 12345,
    "fileType": "image/png"
  }
}
```

### Error Responses

- Returns structured error JSON with details and request metadata.

## Logging

- Logs are written to the console and to `logs/ocr-api-<date>.log`.
- Log level: Debug and above.
- Includes request events, warnings, and errors.
- **Centralized logging:** Logs are also sent to Seq ([http://localhost:5341](http://localhost:5341) by default) for monitoring and troubleshooting. In production, configure the Seq URL in `appsettings.json` or environment variables.

## Testing

- Comprehensive test suite implemented with xUnit, Moq, and Microsoft.AspNetCore.Mvc.Testing
- **Unit Tests**: Test core OCR service logic with mocked dependencies
- **Integration Tests**: Validate API endpoints, file validation, and error handling
- **End-to-End Tests**: Test complete workflows with JWT authentication and file uploads

### Running Tests

```sh
# Run all tests in the solution
dotnet test

# Or specify the solution file explicitly
dotnet test AmharicOCRAPI.sln

# Run tests for the specific test project
dotnet test AmharicOCRAPI.Tests/AmharicOCRAPI.Tests.csproj

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "OcrServiceTests"

# Run tests in watch mode (re-runs on code changes)
dotnet watch test
```

## Security & Production

### ✅ Implemented Security Features

- **JWT Authentication/Authorization**: Bearer token authentication with configurable keys
- **HTTPS Enforcement**: Automatic redirection to secure connections
- **CORS Policies**: Configured for cross-origin requests with specific origins
- **File Upload Validation**: Type, size, and content validation for security
- **Secure Configuration**: Environment variables and user secrets (no hardcoded secrets)

### ✅ Implemented Production Features

- **CI/CD Pipeline**: GitHub Actions workflow for automated building and testing (deployment pending)

### 🔄 Recommended for Production

- **Rate Limiting**: Implement request throttling to prevent abuse (consider ASP.NET Core Rate Limiting)
- **API Versioning**: Add version headers for backward compatibility
- **Health Checks**: Add `/health` endpoint for monitoring service status
- **Security Headers**: Implement security middleware for headers like HSTS, CSP, X-Frame-Options
- **Containerization**: Docker support for consistent deployment

### ⚠️ Important Notes

- Never commit real license keys or secrets to version control
- Use strong, unique JWT signing keys (minimum 256 bits)
- Regularly rotate secrets and monitor for security vulnerabilities
- Consider penetration testing before production deployment

<!-- ## Contributing

Pull requests and issues are welcome! Please open an issue to discuss major changes. -->

## License

<!-- MIT (see LICENSE file) --> Not implemented yet.

## Authors

- [berki6](https://github.com/berki6)
