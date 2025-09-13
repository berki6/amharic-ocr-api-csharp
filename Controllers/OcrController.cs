using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/ocr")]
public class OcrController : ControllerBase
{
    private readonly OcrService _ocrService;

    public OcrController()
    {
        _ocrService = new OcrService();
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractText([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("Image file is required.");

        var tempFilePath = Path.GetTempFileName();

        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var extractedText = _ocrService.ExtractTextFromImage(tempFilePath);

        System.IO.File.Delete(tempFilePath);

        return Ok(new { Text = extractedText });
    }
}
