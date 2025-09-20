namespace FingerPrintVerfication.Entity;

public class FingerPrintAudit : BaseEntity<int>
{
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public FingerPrintAuditType AuditType { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Details { get; set; }
}

public enum FingerPrintAuditType
{
    AddFingerPrint = 1,
    VerifyFingerPrint = 2
}