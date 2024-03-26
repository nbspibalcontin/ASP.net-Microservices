namespace StudentService.DTOs.Response;

public class StudentDto
{
    public Guid StudentId { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public int Age { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Status { get; set; }

    public bool Working { get; set; }

    public string? ImageData { get; set; }
}
