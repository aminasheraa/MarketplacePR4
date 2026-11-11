using API.Models;
using API.DTO;

namespace API.Interfaces
{
    public interface IOrderService
    {
        Task<Result<List<OrderDTO>>> GetAllAsync();
        Task<Result<OrderDTO>> GetByIdAsync(int OrderID);
        Task<Result<List<OrderDTO>>> GetByUserIdAsync(int userID);
        Task<Result<int>> PostListAsync(int userID, List<PostOrderDTO> Products);
    }
}
