using System.Globalization;
using FingerPrintVerfication.Data;
using FingerPrintVerfication.Data.Dtos.Person;
using FingerPrintVerfication.Entity;
using FingerPrintVerfication.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FingerPrintVerfication.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly DataContext _context;

    public PersonRepository(DataContext context)
    {
        _context = context;
    }


    public async Task<GetPersonDto> IsValidPersonWithFingerPrintPathAsync(string path,
        CancellationToken cancellationToken)
    {
        var person = await _context.Persons.FirstOrDefaultAsync(p => p.FingerPrints.Any(e => e.Path == path));
        if (person == null) throw new CultureNotFoundException("Person not found");
        return GetPersonDto.Create(person, null);
    }

    public IQueryable<Person> GetPersonsAsync(CancellationToken cancellationToken)
    {
        return _context.Persons.AsQueryable();
    }

    public async Task<GetPersonDto> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Persons.Include(p => p.FingerPrints).FirstOrDefaultAsync(p => p.Id == id);
        if (user == null) throw new CultureNotFoundException("Person not found");
        return GetPersonDto.Create(user, null);
    }

    public async Task<int> AddPersonAsync(AddPersonRequest request, CancellationToken cancellationToken)
    {
        var person = new Person(request);
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync(cancellationToken);
        return person.Id;
    }

    public Task<int> DeletePersonAsync(int id, CancellationToken cancellationToken)
    {
        var person = _context.Persons.FirstOrDefault(p => p.Id == id);
        if (person == null) throw new CultureNotFoundException("Person not found");
        _context.Persons.Remove(person);
        return _context.SaveChangesAsync(cancellationToken);
    }
}