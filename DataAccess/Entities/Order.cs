using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
