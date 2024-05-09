using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;

namespace SiteWebJO2.Models
{
    /*
     * class ApplicationUser: herited from IdentityUser
     * property Name : user firstname
     * property Lastname : user lastname
     * property Userkey : RandomNumberGenerator, generated with method GenerateUserKey
     */
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "First name")]
        public string Name { get; set; }

        [PersonalData]
        [Display(Name = "Last name")]
        public string Lastname { get; set; }

        public Byte[] Userkey { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<JoTicket> JoTickets { get; set; }


    }

    
}
