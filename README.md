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

3. **Configure your license key:**
   - Edit `appsettings.json` and set your IronOCR license key:

     ```json
     {
       "IronOcr": {
         "LicenseKey": "YOUR_LICENSE_KEY"
       }
     }
     ```

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

- (Recommended) Add unit and integration tests using xUnit and Moq.

## Security & Production

- Add authentication/authorization for public deployments.
- Consider rate limiting and HTTPS enforcement.
- Never commit your real license key or secrets to version control.

<!-- ## Contributing

Pull requests and issues are welcome! Please open an issue to discuss major changes. -->

## License

<!-- MIT (see LICENSE file) --> Not implemented yet.

## Authors

- [berki6](https://github.com/berki6)
