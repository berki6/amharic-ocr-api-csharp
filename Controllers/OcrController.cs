using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/ocr")]
public class OcrController : ControllerBase
{
    private readonly IOcrService _ocrService;

    // Use Dependency Injection to inject OcrService (interface recommended)
    public OcrController(IOcrService ocrService)
    {
        _ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractText([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Image or PDF file is required.");

        var tempFilePath = Path.GetTempFileName();
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outputDir);
        string outputFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".txt";
        string outputFilePath = Path.Combine(outputDir, outputFileName);

        try
        {
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var extractedText = await _ocrService.ExtractTextFromImageAsync(tempFilePath);

            await System.IO.File.WriteAllTextAsync(outputFilePath, extractedText);

            return Ok(new { OutputFile = outputFilePath, Text = extractedText });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
        finally
        {
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
    }
}
