using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string Address { get; set; }

        public DateTime LastLoggedIn { get; set; }

        public string Teacher { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        //Might need to add the user's private key?
    }
}
