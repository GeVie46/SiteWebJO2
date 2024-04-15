using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace SiteWebJO2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string Lastname { get; set; }
        public Byte[] Userkey { get; set; }

        //https://learn.microsoft.com/fr-fr/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-8.0
        public static byte[] GenerateUserKey()
        {
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            return salt;
        }
    }

    
}
