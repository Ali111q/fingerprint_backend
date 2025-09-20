namespace FingerPrintVerfication.Entity;

public class Person : BaseEntity<int>
{
    public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public List<FingerPrint> FingerPrints { get; set; } = [];

    // Parameterless constructor for Entity Framework
    public Person() { }

    // Constructor for creating a person with basic info
    public Person(string fullName, string? companyName = null, string? jobTitle = null)
    {
        FullName = fullName;
        CompanyName = companyName;
        JobTitle = jobTitle;
    }
}