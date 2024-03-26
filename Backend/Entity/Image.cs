using System.ComponentModel.DataAnnotations;

namespace StudentService.Entity;

public class Image
{
    [Key]
    public Guid ImageId { get; set; }
    public byte[]? ImageData { get; set; }
    public Guid StudentId { get; set; }
    public virtual Student? Student { get; set; }
}