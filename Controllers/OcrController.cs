using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/ocr")]

public class OcrController : ControllerBase
{
    private readonly IOcrService _ocrService;
    private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".pdf" };
    private const long maxFileSize = 10 * 1024 * 1024; // 10 MB max

    // Use Dependency Injection to inject OcrService (interface recommended)
    public OcrController(IOcrService ocrService)
    {
        _ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractText([FromForm] IFormFile file)
    {


        if (file == null)
            return BadRequest(new {
                success = false,
                error = "No file was uploaded. Please provide an image or PDF file.",
                data = (object)null,
                request = new {
                    fileName = (string)null,
                    fileSize = (long?)null,
                    fileType = (string)null
                }
            });

        if (file.Length == 0)
            return BadRequest(new {
                success = false,
                error = "The uploaded file is empty. Please provide a valid image or PDF file.",
                data = (object)null,
                request = new {
                    fileName = file.FileName,
                    fileSize = file.Length,
                    fileType = file.ContentType
                }
            });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return BadRequest(new {
                success = false,
                error = $"Unsupported file type: {ext}. Allowed types: {string.Join(", ", allowedExtensions)}",
                data = (object)null,
                request = new {
                    fileName = file.FileName,
                    fileSize = file.Length,
                    fileType = file.ContentType
                }
            });

        if (file.Length > maxFileSize)
            return BadRequest(new {
                success = false,
                error = $"File size exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)} MB.",
                data = (object)null,
                request = new {
                    fileName = file.FileName,
                    fileSize = file.Length,
                    fileType = file.ContentType
                }
            });

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

            return Ok(new {
                success = true,
                error = (string)null,
                data = new {
                    outputFile = outputFilePath,
                    text = extractedText
                },
                request = new {
                    fileName = file.FileName,
                    fileSize = file.Length,
                    fileType = file.ContentType
                }
            });
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
