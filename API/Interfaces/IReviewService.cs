using API.DTO;
using API.Models;

namespace API.Interfaces
{
    public interface IReviewService
    {
        Task<Result<List<ReviewDTO>>> GetAllAsync();
        Task<Result<List<ReviewDTO>>> GetByProductIDAsync(int productID);
        Task<Result<int>> PostAsync(int userID, PostReviewDTO reviewDTO);
        Task<Result<bool>> DeleteAsync(int reviewID);
    }
}
