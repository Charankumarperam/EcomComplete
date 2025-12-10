using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace DataAccess.Entities
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public int Age { get; set; }

        public Customer Customer { get; set; }
    }
}
