using Microsoft.AspNetCore.Identity;

namespace SiteWebJO2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public Byte[] Userkey { get; set; }
    }
}
