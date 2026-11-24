using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CategoryService : ICategoryServise
    {
        private readonly MarketPlacePR4Context _dbContext;

        public CategoryService(MarketPlacePR4Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<CategoryDTO>>> GetAllAsync()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryDTO(
                    c.ID,
                    c.Name
                ))
                .ToListAsync();

            return new Result<List<CategoryDTO>>
            {
                Status = ResultStatus.Success,
                Data = categories
            };
        }

        public async Task<CategoryDTO> CreateAsync(CategoryDTO category)
        {
            var newCategory = new Category
            {
                Name = category.Name
            };

            _dbContext.Categories.Add(newCategory);

            await _dbContext.SaveChangesAsync();

            return new CategoryDTO(
                newCategory.ID,
                newCategory.Name
            );
        }

        public async Task<CategoryDTO> UpdateAsync(int id, CategoryDTO category)
        {
            var existingCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.ID == id);

            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = category.Name;

            await _dbContext.SaveChangesAsync();

            return new CategoryDTO(
                existingCategory.ID,
                existingCategory.Name
            );
        }
    }
}
