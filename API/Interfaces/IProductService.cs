using API.DTO;
using API.Models;

namespace API.Interfaces
{
    public interface IProductService
    {

        Task<Result<List<ProductDTO>>> GetAllAsync();
        Task<Result<ProductDTO>> GetAsync(int ProductID);
        Task<Result<int>> PostAsync(ProductDTO product);
        Task<Result<bool>> PutAsync(int ProductID, ProductDTO product);


    }
}
