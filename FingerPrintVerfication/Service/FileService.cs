using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Takeel.Application.Contracts.File;

namespace Takeel.Infrastructure.Services;

public class FileService : IFileService
{
    private static readonly string FingerprintDirectory = 
        Environment.OSVersion.Platform == PlatformID.Win32NT 
            ? @"C:\fingerprints" 
            : "/app/fingerprints";

    public async Task<string> Upload(FileForm fileForm)
    {
        try
        {
            var id = Guid.NewGuid();
            var extension = Path.GetExtension(fileForm.File.FileName).ToLowerInvariant();
            var fileName = $"{id}{extension}";

            // Create the fingerprints directory if it doesn't exist
            if (!Directory.Exists(FingerprintDirectory))
                Directory.CreateDirectory(FingerprintDirectory);

            var fullPath = Path.Combine(FingerprintDirectory, fileName);

            // Save the file to the specified location
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await fileForm.File.CopyToAsync(stream);
            }

            // Return the relative URL path for HTTP access
            return $"/fingerprints/{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save file: {ex.Message}", ex);
        }
    }

    public async Task<List<string>> Upload(MultiFileForm filesForm)
    {
        var fileList = new List<string>();
        foreach (var file in filesForm.Files)
        {
            var fileToAdd = await Upload(new FileForm { File = file });
            fileList.Add(fileToAdd);
        }

        return fileList;
    }

    public async Task<string> UploadBase64Image(Base64ImageForm base64Form)
    {
        try
        {
            // Remove data URL prefix if present (e.g., "data:image/jpeg;base64,")
            var base64Data = base64Form.Base64Data;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            // Convert base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Determine file extension
            var extension = DetermineImageExtension(imageBytes, base64Form.ImageFormat);
            
            var id = Guid.NewGuid();
            var fileName = $"{Path.GetFileNameWithoutExtension(base64Form.FileName)}_{id}{extension}";

            // Create the fingerprints directory if it doesn't exist
            if (!Directory.Exists(FingerprintDirectory))
                Directory.CreateDirectory(FingerprintDirectory);

            var fullPath = Path.Combine(FingerprintDirectory, fileName);

            // Save the file to the specified location
            await File.WriteAllBytesAsync(fullPath, imageBytes);

            // Return the relative URL path for HTTP access
            return $"/fingerprints/{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save base64 image: {ex.Message}", ex);
        }
    }

    private static string DetermineImageExtension(byte[] imageBytes, string? providedFormat)
    {
        // If format is provided, use it
        if (!string.IsNullOrEmpty(providedFormat))
        {
            var format = providedFormat.ToLowerInvariant();
            if (format.StartsWith("."))
                return format;
            return $".{format}";
        }

        // Try to determine format from byte signature
        if (imageBytes.Length >= 4)
        {
            // JPEG
            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
                return ".jpg";
            
            // PNG
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return ".png";
            
            // GIF
            if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
                return ".gif";
            
            // BMP
            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return ".bmp";
        }

        // WebP (needs more bytes to check)
        if (imageBytes.Length >= 12)
        {
            if (imageBytes[0] == 0x52 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46 && imageBytes[3] == 0x46 &&
                imageBytes[8] == 0x57 && imageBytes[9] == 0x45 && imageBytes[10] == 0x42 && imageBytes[11] == 0x50)
                return ".webp";
        }

        // Default to jpg if cannot determine
        return ".jpg";
    }
}