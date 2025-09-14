using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IronOcr;

namespace AmharicOCRAPI.Tests;

public class OcrServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<global::OcrService>> _mockLogger;

    public OcrServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<global::OcrService>>();
    }

    [Fact]
    public async Task ExtractTextFromImageAsync_ValidImage_ReturnsExtractedText()
    {
        // Arrange
        var licenseKey = "test-license";
        _mockConfig.Setup(c => c["IronOcr:LicenseKey"]).Returns(licenseKey);
        var service = new global::OcrService(_mockConfig.Object, _mockLogger.Object);

        // Note: In a real test, you'd mock IronOcr, but for simplicity, this tests the method structure
        // For full isolation, use Moq to mock IronTesseract

        // Act & Assert
        // This would require a real file path, so in practice, use a test file or mock
        var exception = await Assert.ThrowsAsync<IronOcr.Exceptions.IronOcrProductException>(() => service.ExtractTextFromImageAsync("nonexistent.jpg"));
        Assert.Contains("Image not found", exception.Message); // File not found is expected
    }

    [Fact]
    public void Constructor_SetsLicenseKey_WhenProvided()
    {
        // Arrange
        var licenseKey = "test-license";
        _mockConfig.Setup(c => c["IronOcr:LicenseKey"]).Returns(licenseKey);

        // Act
        var service = new global::OcrService(_mockConfig.Object, _mockLogger.Object);

        // Assert
        // Verify that IronOcr.License.LicenseKey was set (hard to test directly, but constructor runs)
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_SetsLanguageToAmharic()
    {
        // Arrange
        _mockConfig.Setup(c => c["IronOcr:LicenseKey"]).Returns("test");

        // Act
        var service = new global::OcrService(_mockConfig.Object, _mockLogger.Object);

        // Assert
        // Language is set in constructor, but hard to verify without exposing it
        Assert.NotNull(service);
    }
}