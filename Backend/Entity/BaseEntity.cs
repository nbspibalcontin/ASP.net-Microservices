using System.Runtime.InteropServices.JavaScript;

namespace StudentService.Entity;

public class BaseEntity
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public int Age { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}