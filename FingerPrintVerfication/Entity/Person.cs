using FingerPrintVerfication.Data.Dtos.Person;

namespace FingerPrintVerfication.Entity;

public class Person : BaseEntity<int>
{
    public string FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public List<FingerPrint> FingerPrints { get; set; } = [];


    public Person(string fullName, string? companyName, string? jobTitle, List<string> fingerPrints)
    {
        FullName = fullName;
        CompanyName = companyName;
        JobTitle = jobTitle;
        FingerPrints = fingerPrints.Select(fp => new FingerPrint
            {
                Path = fp,
                Person = this,
                PersonId = 0
            }
        ).ToList();
    }

    public Person(AddPersonRequest request)
    {
        FullName = request.FullName;
        CompanyName = request.CompanyName;
        JobTitle = request.JobTitle;
        FingerPrints = request.FingerPrints.Select(fp => new FingerPrint
            {
                Path = fp,
                Person = this,
                PersonId = 0
            }
        ).ToList();
    }
}