using Interfaces.IManagers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace EcomComplete.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;
        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result= await _productManager.GetAllAsync();
            
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [Authorize(Policy = "MinimumAgePolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _productManager.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [Authorize(Policy="AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(CreateProduct dto)
        {
           var result= await _productManager.AddAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(dto);
        }
        [Authorize(Policy ="AdminPolicy")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(UpdateProduct dto)
        {
            var result = await _productManager.UpdateAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(dto);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await _productManager.DeleteAsync(id);
            return result.Success ? Ok(result) : Unauthorized(result);
        }


    }
}
