
using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private MarketPlacePR4Context _dbContext;

        public ProductService(MarketPlacePR4Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<ProductDTO>>> GetAllAsync()
        {
            List<ProductDTO> products = await _dbContext.Products.Select(p => new ProductDTO(
                    p.ID,
                    p.Name,
                    p.Category.ID,
                    p.Category.Name,
                    p.Description,
                    p.Price,
                    p.Image,
                    p.Rating,
                    p.PackageCount ?? 0,
                    p.IsAvailable,
                    p.StockQuantity
                ))
                .ToListAsync();

            return new Result<List<ProductDTO>>
            {
                Status = ResultStatus.Success,
                Data = products
            };
        }

        public async Task<Result<ProductDTO>> GetAsync(int ProductID)
        {
            ProductDTO? product = await _dbContext.Products.Where(p => p.ID == ProductID).Select(p => new ProductDTO(
                    p.ID,
                    p.Name,
                    p.Category.ID,
                    p.Category.Name,
                    p.Description,
                    p.Price,
                    p.Image,
                    p.Rating,
                    p.PackageCount ?? 0,
                    p.IsAvailable,
                    p.StockQuantity
                ))
                .FirstOrDefaultAsync();

            if (product == null)
                return new Result<ProductDTO>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар по ID не найден"
                };

            return new Result<ProductDTO>
            {
                Status = ResultStatus.Success,
                Data = product
            };
        }

        public async Task<Result<int>> PostAsync(ProductDTO product)
        {
            if (product == null)
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Данные товара не указаны"
                };

            if (string.IsNullOrWhiteSpace(product.Name) ||
                product.Price < 0 ||
                product.StockQuantity < 0 ||
                product.PackageCount < 0)
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Некорректные данные товара"
                };

            bool categoryExists = await _dbContext.Categories
                .AnyAsync(c => c.ID == product.CategoryID);

            if (!categoryExists)
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Категория не найдена"
                };

            Product newProduct = new Product
            {
                Name = product.Name,
                CategoryID = product.CategoryID,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image,
                PackageCount = product.PackageCount,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            };

            _dbContext.Products.Add(newProduct);

            await _dbContext.SaveChangesAsync();

            return new Result<int>
            {
                Status = ResultStatus.Success,
                Data = newProduct.ID
            };
        }

        public async Task<Result<bool>> PutAsync(int ProductID, ProductDTO product)
        {
            if (product == null)
                return new Result<bool>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Данные товара не указаны"
                };

            Product? existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ID == ProductID);

            if (existingProduct == null)
                return new Result<bool>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар по ID не найден"
                };

            if (string.IsNullOrWhiteSpace(product.Name) ||
                product.Price < 0 ||
                product.StockQuantity < 0 ||
                product.PackageCount < 0)
                return new Result<bool>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Некорректные данные товара"
                };

            bool categoryExists = await _dbContext.Categories
                .AnyAsync(c => c.ID == product.CategoryID);

            if (!categoryExists)
                return new Result<bool>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Категория не найдена"
                };

            existingProduct.Name = product.Name;
            existingProduct.CategoryID = product.CategoryID;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Image = product.Image;
            existingProduct.PackageCount = product.PackageCount;
            existingProduct.IsAvailable = product.IsAvailable;
            existingProduct.StockQuantity = product.StockQuantity;

            await _dbContext.SaveChangesAsync();

            return new Result<bool>
            {
                Status = ResultStatus.Success,
                Data = true
            };
        }
    }
}
