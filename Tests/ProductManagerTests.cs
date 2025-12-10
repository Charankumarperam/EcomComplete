using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Interfaces.IRepository;
using Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Models.DTOs;
using DataAccess.Entities;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ProductManagerTests
    {
        private Mock<IGenericRepository<Product>> _repoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<ProductManager>> _loggerMock;
        private IMemoryCache _memoryCache;
        private ProductManager _manager;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Product>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProductManager>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _manager = new ProductManager(
                _repoMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _memoryCache
            );
        }

        [Test]
        public async Task GetAllAsync_ReturnsProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { ProductId = 1, ProductName = "Laptop" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IList<ProductDto>>(products))
                       .Returns(new List<ProductDto> { new ProductDto { ProductId = 1, ProductName = "Laptop" } });

            // Act
            var result = await _manager.GetAllAsync();

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Count);
            Assert.AreEqual("Laptop", result.Data[0].ProductName);
        }

        [Test]
        public async Task GetByIdAsync_ProductExists_ReturnsProduct()
        {
            var product = new Product { ProductId = 1, ProductName = "Phone" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                       .Returns(new ProductDto { ProductId = 1, ProductName = "Phone" });

            var result = await _manager.GetByIdAsync(1);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Phone", result.Data.ProductName);
        }

        [Test]
        public async Task GetByIdAsync_ProductNotFound_ReturnsFailure()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

            var result = await _manager.GetByIdAsync(1);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Product not found", result.Message);
        }

        [Test]
        public async Task AddAsync_AddsProductSuccessfully()
        {
            var dto = new CreateProduct { ProductName = "Tablet" };
            var product = new Product { ProductName = "Tablet" };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _repoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);

            var result = await _manager.AddAsync(dto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Product added successfully", result.Message);
        }

        [Test]
        public async Task UpdateAsync_ProductExists_UpdatesSuccessfully()
        {
            var product = new Product { ProductId = 1, ProductName = "OldName" };
            var dto = new UpdateProduct { ProductId = 1, ProductName = "NewName" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map(dto, product));
            _repoMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

            var result = await _manager.UpdateAsync(dto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Product updated successfully", result.Message);
        }

        [Test]
        public async Task UpdateAsync_ProductNotFound_ReturnsFailure()
        {
            var dto = new UpdateProduct { ProductId = 1, ProductName = "NewName" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

            var result = await _manager.UpdateAsync(dto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Error updating product: Product not found", result.Message);
        }

        [Test]
        public async Task DeleteAsync_ProductExists_DeletesSuccessfully()
        {
            var product = new Product { ProductId = 1, ProductName = "Laptop" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _repoMock.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

            var result = await _manager.DeleteAsync(1);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Product deleted successfully", result.Message);
        }

        [Test]
        public async Task DeleteAsync_ProductNotFound_ReturnsFailure()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

            var result = await _manager.DeleteAsync(1);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Product not found", result.Message);
        }
    }
}