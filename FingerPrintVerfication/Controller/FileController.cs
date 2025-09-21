
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takeel.Application.Contracts.File;

namespace Takeel.WebApi.Controllers;


[Route("api/file")]
[ApiController]
public class FileController: ControllerBase{
    
    
    private readonly IFileService _fileService;

    public FileController(IFileService fileService) {
        _fileService = fileService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Upload([FromForm] FileForm fileForm) =>  Ok(await _fileService.Upload(fileForm));
    
    [HttpPost("multi")]
    [AllowAnonymous]
    public async Task<IActionResult> Upload([FromForm] MultiFileForm filesForm) => Ok(await _fileService.Upload(filesForm));
    
    
    /// <summary>
    /// Upload an image from base64 data
    /// </summary>
    /// <param name="base64Form">Base64 image data with filename and optional format</param>
    /// <returns>URL path to the saved image</returns>
    [HttpPost("base64")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadBase64Image([FromBody] Base64ImageForm base64Form)
    {
        try
        {
            if (string.IsNullOrEmpty(base64Form.Base64Data))
            {
                return BadRequest(new { message = "Base64 data is required" });
            }

            if (string.IsNullOrEmpty(base64Form.FileName))
            {
                return BadRequest(new { message = "Filename is required" });
            }

            var filePath = await _fileService.UploadBase64Image(base64Form);
            return Ok(new { 
                message = "Image uploaded successfully", 
                filePath = filePath,
                fileName = Path.GetFileName(filePath)
            });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Invalid base64 format" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to upload image", error = ex.Message });
        }
    }
    
 
    [HttpGet("download/{fileName}")]
    [AllowAnonymous]
    public IActionResult Download(string fileName)
    {
        // Validate the input to avoid path traversal attacks
        fileName = Path.GetFileName(fileName); // strips any path parts

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "attachments", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var contentType = GetContentType(fileName);
        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, contentType, fileName);
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }
}