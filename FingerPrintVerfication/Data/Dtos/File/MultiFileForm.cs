using Microsoft.AspNetCore.Http;

namespace Takeel.Application.Contracts.File;
public class MultiFileForm
{
    public List<IFormFile> Files { get; set; }
}
