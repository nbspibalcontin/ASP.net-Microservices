using Microsoft.AspNetCore.Identity;

namespace Frontend.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName {  get; set; }
        public int Age { get; set; }
    }
}
