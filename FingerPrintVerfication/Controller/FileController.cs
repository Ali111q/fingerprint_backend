
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takeel.Application.Contracts.File;

namespace Takeel.WebApi.Controllers;


[Route("api/file")]
[ApiController]
[Authorize]
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