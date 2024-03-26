using System.ComponentModel.DataAnnotations;

namespace StudentService.Entity;

public class Student : BaseEntity
{
    [Key]
    public Guid StudentId { get; set; }
    public string? Status { get; set; }
    public bool Working { get; set; } 
    public virtual Image? Images { get; set; }
}