namespace FingerPrintVerfication.Data.Dtos.Person;

public class GetPersonDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public float? Similarity { get; set; }
    public int FingerPrintCount { get; set; }

    public static GetPersonDto Create(Entity.Person person,
        float? similarity) =>
        new GetPersonDto
        {
            Id = person.Id,
            FullName = person.FullName,
            CompanyName = person.CompanyName,
            JobTitle = person.JobTitle,
            Similarity = similarity,
            FingerPrintCount = person.FingerPrints?.Count ?? 0
        };
}