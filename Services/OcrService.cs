using IronOcr;
using Microsoft.Extensions.Configuration;

public class OcrService : IOcrService
{
    private readonly IronTesseract OcrEngine;

    public OcrService(IConfiguration configuration)
    {
        var licenseKey = configuration["IronOcr:LicenseKey"];
        if (!string.IsNullOrEmpty(licenseKey))
        {
            IronOcr.License.LicenseKey = licenseKey;
        }
        OcrEngine = new IronTesseract();
        OcrEngine.Language = OcrLanguage.Amharic;
    }

    public async Task<string> ExtractTextFromImageAsync(string imagePath)
    {
        return await Task.Run(() =>
        {
            using var input = new OcrInput();
            input.LoadImage(imagePath);
            var result = OcrEngine.Read(input);
            return result.Text;
        });
    }
}
