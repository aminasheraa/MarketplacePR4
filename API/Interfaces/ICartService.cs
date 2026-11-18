using API.DTO;
using API.Models;

namespace API.Interfaces
{
    public interface ICartService
    {
        Task<Result<List<CartItemDTO>>> GetByUserIdAsync(int userID);

        Task<Result<int>> AddAsync(int userID, PostOrderDTO product);

        Task<Result<bool>> UpdateQuantityAsync(int userID, int productCartID, int quantity);

        Task<Result<bool>> DeleteAsync(int userID, int productCartID);
    }
}
