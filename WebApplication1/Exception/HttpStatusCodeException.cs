namespace WebApplication1.Exception
{
    public class HttpStatusCodeException : System.Exception
    {
        public HttpStatusCodeException()
        {
        }

        public HttpStatusCodeException(string message) : base(message)
        {
        }

        public HttpStatusCodeException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
