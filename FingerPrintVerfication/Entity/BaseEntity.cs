namespace FingerPrintVerfication.Entity;

public class BaseEntity<TId>
{
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}