namespace AuthenticationService.Exception
{
    public class RoleAlreadyExist : System.Exception
    {
        public RoleAlreadyExist()
        {
        }

        public RoleAlreadyExist(string message)
            : base(message)
        {
        }

        public RoleAlreadyExist(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
