using IronOcr;

public class OcrService
{
    private IronTesseract OcrEngine;

    public OcrService()
    {
        OcrEngine = new IronTesseract();
        OcrEngine.Language = OcrLanguage.Amharic;
    }

    public string ExtractTextFromImage(string imagePath)
    {
        using var input = new OcrInput(imagePath);
        var result = OcrEngine.Read(input);
        return result.Text;
    }
}
