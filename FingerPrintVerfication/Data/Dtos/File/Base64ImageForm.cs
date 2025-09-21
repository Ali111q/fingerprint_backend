using System.ComponentModel.DataAnnotations;

namespace Takeel.Application.Contracts.File;

public class Base64ImageForm
{
    [Required]
    public required string Base64Data { get; set; }
    
    [Required]
    public required string FileName { get; set; }
    
    /// <summary>
    /// Image format (jpg, png, gif, bmp, webp). If not provided, will be inferred from base64 data or default to jpg
    /// </summary>
    public string? ImageFormat { get; set; }
}
