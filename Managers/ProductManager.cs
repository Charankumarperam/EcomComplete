using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IManagers;
using Models.DTOs;
using Interfaces.IRepository;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
namespace Managers
{
    public class ProductManager:IProductManager
    {
        private readonly IGenericRepository<Product> _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductManager> _logger;
        private readonly IMemoryCache _cache;
        
        public ProductManager(IGenericRepository<Product> repo, ILogger<ProductManager> logger,IMapper mapper, IMemoryCache cache)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
            _cache= cache;
        }
        public async Task<Result<IList<ProductDto>>> GetAllAsync()
        {
            const string cacheKey = "all_products";
            if (!_cache.TryGetValue(cacheKey, out IList<ProductDto> productDtos))
            {
                var products= await _repo.GetAllAsync();
                productDtos= _mapper.Map<IList<ProductDto>>(products);

                //configure cache options
                var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, productDtos, cacheOptions);
                _logger.LogInformation("Products retrieved from database and cached");

            }
            else
            {
                 _logger.LogInformation("Products retrieved from cache");
            }
                
            return new Result<IList<ProductDto>> {Success=true,Message="Products",Data= productDtos };

        }
        public async Task<Result<ProductDto>> GetByIdAsync(int id)
        {
           
                string cacheKey = $"product_{id}";
            if (!_cache.TryGetValue(cacheKey, out ProductDto productDto))
            {
                var product = await _repo.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with id {ProductId} not found", id);
                    return new Result<ProductDto> { Success = false, Message = "Product not found" };
                }
                productDto = _mapper.Map<ProductDto>(product);
                var cacheOptions = new MemoryCacheEntryOptions()
           .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, productDto, cacheOptions);

                _logger.LogInformation("Product retrieved from database and cached");

            }
            else
            {
                _logger.LogInformation("Product retrieved from cache");
            }


           return new Result<ProductDto> { Success = true, Message = "Product found", Data = productDto };
        }
        public async Task<Result> AddAsync(CreateProduct dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            //invalidate cache
            _cache.Remove("all_products");

            _logger.LogInformation("Product added successfully and cache invalidation done");
            return new Result { Success = true, Message = "Product added successfully" };
        }
        public async Task<Result> UpdateAsync(UpdateProduct dto)
        {
                var product = await _repo.GetByIdAsync(dto.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product with id {ProductId} not found for update", dto.ProductId);
                    throw new Exception("Product not found");
                }
                _mapper.Map(dto, product);
                await _repo.UpdateAsync(product);
                //invalidate cache for this product and all products
                _cache.Remove($"product_{dto.ProductId}");
                _cache.Remove("all_products");
           
                _logger.LogInformation("Product updated successfully and cache invalidation done");
                return new Result { Success = true, Message = "Product updated successfully" };
            
           
        }
        public async Task<Result> DeleteAsync(int id)
        {
            
                var product = await _repo.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with id {ProductId} not found for deletion", id);
                    return new Result { Success = false, Message = "Product not found" };
                }
                await _repo.DeleteAsync(product);
            // invalidate cache
            _cache.Remove($"product_{id}");
            _cache.Remove("all_products");


            _logger.LogInformation("Product deleted successfully and invalidation of cache done");
                return new Result { Success = true, Message = "Product deleted successfully" };
            
        }
    }
}
