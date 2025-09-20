using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Takeel.Application.Contracts.File;

namespace Takeel.Infrastructure.Services;

public class FileService : IFileService
{
    public async Task<string> Upload(FileForm fileForm)
    {
        try
        {
            var id = Guid.NewGuid();
            var extension = Path.GetExtension(fileForm.File.FileName).ToLowerInvariant();
            var fileName = $"{id}{extension}";

            var attachmentsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
            if (!Directory.Exists(attachmentsDir))
                Directory.CreateDirectory(attachmentsDir);

            var path = Path.Combine(attachmentsDir, fileName);


            var filePath = Path.Combine("Attachments", fileName);
            return filePath;
        }
        catch (Exception)
        {
            throw;
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