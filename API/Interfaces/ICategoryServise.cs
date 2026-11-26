using API.DTO;
using API.Models;

namespace API.Interfaces
{
    public interface ICategoryServise
    {
        Task<Result<List<CategoryDTO>>> GetAllAsync();

        Task<CategoryDTO> CreateAsync(CategoryDTO category);

        Task<CategoryDTO> UpdateAsync(int id, CategoryDTO category);
    }
}
