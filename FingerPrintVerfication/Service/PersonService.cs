using FingerPrintVerfication.Data.Dtos.Person;
using FingerPrintVerfication.Entity;
using Takeel.Application.Contracts;

namespace Takeel.Infrastructure.Services;

public interface IPersonService
{
    Task<GetPersonDto> IsValidPersonWithFingerPrintPathAsync(string path, CancellationToken cancellationToken);
    Task<PaginatedResponse<Person>> GetPersonsAsync(CancellationToken cancellationToken);
    Task<GetPersonDto> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddPersonAsync(AddPersonRequest request, CancellationToken cancellationToken);
    Task<int> DeletePersonAsync(int id, CancellationToken cancellationToken);
}

public class PersonService
{
    
}