using Microsoft.AspNetCore.Identity;

namespace PracticeWebApp.Models
{
    public class User : IdentityUser
    {
        public string Role { get; set; }
    }
}
