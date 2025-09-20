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
}