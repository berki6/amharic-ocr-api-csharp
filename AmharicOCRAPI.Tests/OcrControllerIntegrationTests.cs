using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using AmharicOCRAPI;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace AmharicOCRAPI.Tests;

public class OcrControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OcrControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Override configuration for tests
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Seq:Url"] = "" // Disable Seq logging for tests
                });
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ExtractText_PostWithoutFile_ReturnsBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();

        // Act
        var response = await _client.PostAsync("/api/ocr/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Note: ASP.NET Core returns automatic validation error for missing file
    }

    [Fact]
    public async Task ExtractText_PostWithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[0]);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "empty.png");

        // Act
        var response = await _client.PostAsync("/api/ocr/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Note: ASP.NET Core returns automatic validation error for empty file
    }

    [Fact]
    public async Task ExtractText_PostWithUnsupportedFileType_ReturnsBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await _client.PostAsync("/api/ocr/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Note: ASP.NET Core returns automatic validation error for unsupported file type
    }

    [Fact]
    public async Task ExtractText_PostWithLargeFile_ReturnsBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var largeFile = new byte[11 * 1024 * 1024]; // 11 MB
        var fileContent = new ByteArrayContent(largeFile);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "large.png");

        // Act
        var response = await _client.PostAsync("/api/ocr/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Note: ASP.NET Core returns automatic validation error for oversized file
    }

    // Note: For successful extraction, you'd need a real image file and JWT token
    // This would require setting up test data and authentication
}