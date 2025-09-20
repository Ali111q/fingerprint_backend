using FingerPrintVerfication.Data.Dtos.Person;
using FingerPrintVerfication.Entity;
using Takeel.Application.Contracts;

namespace FingerPrintVerfication.Interfaces;

public interface IPersonRepository
{
    Task<GetPersonDto> IsValidPersonWithFingerPrintPathAsync(string path, CancellationToken cancellationToken);
    IQueryable<Person> GetPersonsAsync(CancellationToken cancellationToken);
    Task<GetPersonDto> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddPersonAsync(AddPersonRequest request, CancellationToken cancellationToken);
    Task<int> DeletePersonAsync(int id, CancellationToken cancellationToken);
}