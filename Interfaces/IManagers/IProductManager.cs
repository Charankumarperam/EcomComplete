using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IProductManager
    {
        Task<Result<IList<ProductDto>>> GetAllAsync();
        Task<Result<ProductDto>> GetByIdAsync(int id);
        Task<Result> AddAsync(CreateProduct dto);
        Task<Result> UpdateAsync(UpdateProduct dto);
        Task<Result> DeleteAsync(int id);
    }
}
