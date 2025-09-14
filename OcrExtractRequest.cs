using Microsoft.AspNetCore.Http;

public class OcrExtractRequest
{
    public IFormFile? File { get; set; }
}
