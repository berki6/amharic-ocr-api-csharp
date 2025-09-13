using IronOcr;
using Microsoft.Extensions.Configuration;

public class OcrService : IOcrService
{
    private readonly IronTesseract OcrEngine;
    private readonly ILogger<OcrService> _logger;

    public OcrService(IConfiguration configuration, ILogger<OcrService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var licenseKey = configuration["IronOcr:LicenseKey"];
        if (!string.IsNullOrEmpty(licenseKey))
        {
            IronOcr.License.LicenseKey = licenseKey;
        }
        OcrEngine = new IronTesseract();
        OcrEngine.Language = OcrLanguage.Amharic;
    }

    public async Task<string> ExtractTextFromImageAsync(string filePath)
    {
        try
        {
            _logger.LogInformation("Starting OCR extraction for file '{FilePath}'.", filePath);
            return await Task.Run(() =>
            {
                using var input = new OcrInput();

                // Load the input file (image or PDF)
                if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    input.LoadPdf(filePath);
                else
                    input.LoadImage(filePath);

                // Optional image preprocessing to improve recognition accuracy
                input.Deskew();         // Corrects tilted images
                input.DeNoise();        // Removes background noise
                input.EnhanceResolution(); // Improves blurry scans

                var result = OcrEngine.Read(input);
                _logger.LogInformation("OCR extraction completed for file '{FilePath}'.", filePath);
                return result.Text;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OCR extraction for file '{FilePath}'.", filePath);
            throw;
        }
    }
}
