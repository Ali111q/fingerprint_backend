namespace FingerPrintVerfication.Entity;

public class FingerPrint : BaseEntity<int>
{
    public required string Path { get; set; }
    public required int PersonId { get; set; }
    public Person Person { get; set; }
}