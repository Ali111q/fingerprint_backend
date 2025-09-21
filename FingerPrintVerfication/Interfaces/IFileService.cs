using Takeel.Application.Contracts.File;

public interface IFileService {
    Task<string >  Upload(FileForm fileForm);
    Task<List<string> > Upload(MultiFileForm filesForm);
    Task<string> UploadBase64Image(Base64ImageForm base64Form);
}