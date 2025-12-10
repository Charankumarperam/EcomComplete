using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public IList<Order> Orders { get; set; }= new List<Order>();
    }
}
