using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/ocr")]
[Authorize]
// [Authorize] // Disable for testing - authentication can be added back when needed

public class OcrController : ControllerBase
{
    private readonly IOcrService _ocrService;
    private readonly ILogger<OcrController> _logger;
    private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".pdf" };
    private const long maxFileSize = 10 * 1024 * 1024; // 10 MB max

    // Use Dependency Injection to inject OcrService (interface recommended)
    public OcrController(IOcrService ocrService, ILogger<OcrController> logger)
    {
        _ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractText([FromForm] OcrExtractRequest request)
    {

        var file = request.File;

        if (file == null)
        {
            _logger.LogWarning("No file was uploaded in the request.");
            return BadRequest(new {
                success = false,
                error = "No file was uploaded. Please provide an image or PDF file.",
                data = string.Empty,
                request = new {
                    fileName = string.Empty,
                    fileSize = 0L,
                    fileType = string.Empty
                }
            });
        }

        if (file.Length == 0)
        {
            _logger.LogWarning("Uploaded file '{FileName}' is empty.", file.FileName);
            return BadRequest(new {
                success = false,
                error = "The uploaded file is empty. Please provide a valid image or PDF file.",
                data = string.Empty,
                request = new {
                    fileName = file.FileName ?? string.Empty,
                    fileSize = file.Length,
                    fileType = file.ContentType ?? string.Empty
                }
            });
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            _logger.LogWarning("Unsupported file type '{FileType}' uploaded.", ext);
            return BadRequest(new {
                success = false,
                error = $"Unsupported file type: {ext}. Allowed types: {string.Join(", ", allowedExtensions)}",
                data = string.Empty,
                request = new {
                    fileName = file.FileName ?? string.Empty,
                    fileSize = file.Length,
                    fileType = file.ContentType ?? string.Empty
                }
            });
        }

        if (file.Length > maxFileSize)
        {
            _logger.LogWarning("File '{FileName}' exceeds the maximum allowed size.", file.FileName);
            return BadRequest(new {
                success = false,
                error = $"File size exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)} MB.",
                data = string.Empty,
                request = new {
                    fileName = file.FileName ?? string.Empty,
                    fileSize = file.Length,
                    fileType = file.ContentType ?? string.Empty
                }
            });
        }

        var tempFilePath = Path.GetTempFileName();
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outputDir);
        string outputFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".txt";
        string outputFilePath = Path.Combine(outputDir, outputFileName);

        try
        {
            _logger.LogInformation("Processing file '{FileName}' for OCR.", file.FileName);
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var extractedText = await _ocrService.ExtractTextFromImageAsync(tempFilePath);

            await System.IO.File.WriteAllTextAsync(outputFilePath, extractedText);

            _logger.LogInformation("OCR extraction completed for file '{FileName}'. Output saved to '{OutputFile}'.", file.FileName, outputFilePath);
            return Ok(new {
                success = true,
                error = string.Empty,
                data = new {
                    outputFile = outputFilePath,
                    text = extractedText
                },
                request = new {
                    fileName = file.FileName ?? string.Empty,
                    fileSize = file.Length,
                    fileType = file.ContentType ?? string.Empty
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing file '{FileName}'.", file?.FileName);
            return StatusCode(500, new {
                success = false,
                error = ex.Message ?? string.Empty,
                data = string.Empty,
                request = new {
                    fileName = file?.FileName ?? string.Empty,
                    fileSize = file?.Length ?? 0L,
                    fileType = file?.ContentType ?? string.Empty
                }
            });
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
