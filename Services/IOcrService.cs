public interface IOcrService
{
    Task<string> ExtractTextFromImageAsync(string imagePath);
}