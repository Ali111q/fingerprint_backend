namespace FingerPrintVerfication.Data.Dtos.Person;

public class AddPersonRequest
{
    public required string FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public List<string> FingerPrints { get; set; } = [];
}