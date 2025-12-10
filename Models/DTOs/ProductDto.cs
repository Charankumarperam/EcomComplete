using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IList<OrderItemDto>? OrderItems { get; set; } = new List<OrderItemDto>();
    }
    public class CreateProduct
    {
       
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

    }
    public class UpdateProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

    }
}
