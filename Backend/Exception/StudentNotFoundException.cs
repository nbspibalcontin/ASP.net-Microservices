namespace StudentService.Exception;

public class StudentNotFoundException : System.Exception
{
    public StudentNotFoundException()
    {
    }

    public StudentNotFoundException(string message) : base(message)
    {
    }

    public StudentNotFoundException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
    
}