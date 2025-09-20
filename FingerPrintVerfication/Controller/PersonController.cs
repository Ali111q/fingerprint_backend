using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FingerPrintVerfication.Data.Dtos.Person;
using FingerPrintVerfication.Interfaces;
using FingerPrintVerfication.Entity;
using FingerPrintVerfication.Data.Dtos;
using FingerPrintVerfication.Data;
using Takeel.Application.Contracts;
using System.Globalization;

namespace FingerPrintVerfication.Controller;

[Route("api/person")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IPersonRepository _personRepository;
    private readonly DataContext _context;

    public PersonController(IPersonRepository personRepository, DataContext context)
    {
        _personRepository = personRepository;
        _context = context;
    }

    /// <summary>
    /// Add a new person with 5 fingerprints
    /// </summary>
    /// <param name="request">Person details with exactly 5 fingerprint paths</param>
    /// <returns>Created person ID</returns>
    [HttpPost]
    public async Task<IActionResult> AddPerson([FromBody] AddPersonRequest request)
    {
        try
        {
            // Validate that exactly 5 fingerprints are provided
            if (request.FingerPrints == null || request.FingerPrints.Count != 5)
            {
                return BadRequest(new { message = "Exactly 5 fingerprints are required for each person" });
            }

            // Validate that all fingerprint paths are provided
            if (request.FingerPrints.Any(string.IsNullOrWhiteSpace))
            {
                return BadRequest(new { message = "All fingerprint paths must be provided" });
            }

            var personId = await _personRepository.AddPersonAsync(request, CancellationToken.None);

            // Log audit entry for adding person
            await LogAuditEntry(0, FingerPrintAuditType.AddFingerPrint, true, 
                $"Person '{request.FullName}' added successfully with 5 fingerprints");

            return Ok(new { id = personId, message = "Person added successfully" });
        }
        catch (Exception ex)
        {
            // Log failed audit entry
            await LogAuditEntry(0, FingerPrintAuditType.AddFingerPrint, false, 
                $"Failed to add person: {ex.Message}");

            return StatusCode(500, new { message = "Failed to add person", error = ex.Message });
        }
    }

    /// <summary>
    /// Verify person by fingerprint path
    /// </summary>
    /// <param name="fingerprintPath">Path to the fingerprint file</param>
    /// <returns>Person details if found</returns>
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyPerson([FromQuery] string fingerprintPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fingerprintPath))
            {
                return BadRequest(new { message = "Fingerprint path is required" });
            }

            var person = await _personRepository.IsValidPersonWithFingerPrintPathAsync(fingerprintPath, CancellationToken.None);

            // Log successful verification audit entry
            await LogAuditEntry(person.Id, FingerPrintAuditType.VerifyFingerPrint, true, 
                $"Person '{person.FullName}' verified successfully using fingerprint path: {fingerprintPath}");

            return Ok(person);
        }
        catch (CultureNotFoundException)
        {
            // Log failed verification audit entry
            await LogAuditEntry(0, FingerPrintAuditType.VerifyFingerPrint, false, 
                $"Verification failed for fingerprint path: {fingerprintPath}");

            return NotFound(new { message = "Person not found with the provided fingerprint" });
        }
        catch (Exception ex)
        {
            // Log failed audit entry
            await LogAuditEntry(0, FingerPrintAuditType.VerifyFingerPrint, false, 
                $"Verification error: {ex.Message}");

            return StatusCode(500, new { message = "Verification failed", error = ex.Message });
        }
    }

    /// <summary>
    /// Get audit logs for fingerprint operations
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="auditType">Filter by audit type (optional)</param>
    /// <param name="isSuccessful">Filter by success status (optional)</param>
    /// <param name="userId">Filter by user ID (optional)</param>
    /// <returns>Paginated audit logs</returns>
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] FingerPrintAuditType? auditType = null,
        [FromQuery] bool? isSuccessful = null,
        [FromQuery] int? userId = null)
    {
        try
        {
            var query = _context.FingerPrintAudits.AsQueryable();

            // Apply filters
            if (auditType.HasValue)
                query = query.Where(a => a.AuditType == auditType.Value);

            if (isSuccessful.HasValue)
                query = query.Where(a => a.IsSuccessful == isSuccessful.Value);

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            // Order by timestamp descending (newest first)
            query = query.OrderByDescending(a => a.Timestamp);

            // Apply pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.Id,
                    a.UserId,
                    a.Timestamp,
                    a.AuditType,
                    a.IsSuccessful,
                    a.Details
                })
                .ToListAsync();

            var response = new PaginatedResponse<object>(items, page, (int)Math.Ceiling((double)totalCount / pageSize), totalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to retrieve audit logs", error = ex.Message });
        }
    }

    /// <summary>
    /// Get all persons with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of persons</returns>
    [HttpGet]
    public async Task<IActionResult> GetPersons([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _personRepository.GetPersonsAsync(CancellationToken.None);
            var totalCount = await query.CountAsync();
            
            var items = await query
                .Include(p => p.FingerPrints)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetPersonDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    CompanyName = p.CompanyName,
                    JobTitle = p.JobTitle,
                    FingerPrintCount = p.FingerPrints.Count
                })
                .ToListAsync();

            var response = new PaginatedResponse<GetPersonDto>(items, page, (int)Math.Ceiling((double)totalCount / pageSize), totalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to retrieve persons", error = ex.Message });
        }
    }

    /// <summary>
    /// Get person by ID
    /// </summary>
    /// <param name="id">Person ID</param>
    /// <returns>Person details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPersonById(int id)
    {
        try
        {
            var person = await _personRepository.GetPersonByIdAsync(id, CancellationToken.None);
            return Ok(person);
        }
        catch (CultureNotFoundException)
        {
            return NotFound(new { message = "Person not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to retrieve person", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete person by ID
    /// </summary>
    /// <param name="id">Person ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            await _personRepository.DeletePersonAsync(id, CancellationToken.None);
            
            // Log audit entry for deletion
            await LogAuditEntry(id, FingerPrintAuditType.AddFingerPrint, true, 
                $"Person with ID {id} deleted successfully");

            return Ok(new { message = "Person deleted successfully" });
        }
        catch (CultureNotFoundException)
        {
            return NotFound(new { message = "Person not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to delete person", error = ex.Message });
        }
    }

    private async Task LogAuditEntry(int userId, FingerPrintAuditType auditType, bool isSuccessful, string details)
    {
        try
        {
            var audit = new FingerPrintAudit
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                AuditType = auditType,
                IsSuccessful = isSuccessful,
                Details = details
            };

            _context.FingerPrintAudits.Add(audit);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Silently fail audit logging to not break main operations
        }
    }
}
