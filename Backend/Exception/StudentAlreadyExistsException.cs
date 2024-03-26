namespace Backend.Exception
{
    public class StudentAlreadyExistsException : System.Exception
    {
        public StudentAlreadyExistsException()
        {
        }

        public StudentAlreadyExistsException(string message)
            : base(message)
        {
        }

        public StudentAlreadyExistsException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
