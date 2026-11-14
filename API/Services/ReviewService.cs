using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ReviewService : IReviewService
    {
        private MarketPlacePR4Context _dbContext;

        public ReviewService(MarketPlacePR4Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<ReviewDTO>>> GetAllAsync()
        {
            List<ReviewDTO> reviews = await _dbContext.Reviews
                .Select(r => new ReviewDTO(
                    r.ID,
                    r.Date,
                    r.Rating,
                    r.Description,
                    r.User.Name,
                    r.Product.Name
                    )
                ).ToListAsync();

            return new Result<List<ReviewDTO>>
            {
                Status = ResultStatus.Success,
                Data = reviews
            };
        }

        public async Task<Result<List<ReviewDTO>>> GetByProductIDAsync(int productID)
        {
            if (!await _dbContext.Products.AnyAsync(p => p.ID == productID))
            {
                return new Result<List<ReviewDTO>>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Такого товара не существует."
                };
            }

            List<ReviewDTO>? productReviews = await _dbContext.Reviews.Where(rev => rev.ProductID == productID)
                .Select(r => new ReviewDTO(
                    r.ID,
                    r.Date,
                    r.Rating,
                    r.Description,
                    r.User.Name,
                    r.Product.Name)
                ).ToListAsync();

            return new Result<List<ReviewDTO>>
            {
                Status = ResultStatus.Success,
                Data = productReviews
            };
        }

        public async Task<Result<int>> PostAsync(int userID, PostReviewDTO reviewDTO)
        {
            if (userID <= 0 || !await _dbContext.Users.AnyAsync(u => u.ID == userID))
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Пользователь не найден."
                };

            if (!await _dbContext.Products.AnyAsync(p => p.ID == reviewDTO.ProductID))
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар не найден."
                };

            if (reviewDTO.Rating < 1 || reviewDTO.Rating > 5)
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Оценка должна быть от 1 до 5."
                };

            if (string.IsNullOrWhiteSpace(reviewDTO.Description))
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Текст отзыва не может быть пустым."
                };

            Review review = new Review 
            { 
                UserID = userID,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Rating = reviewDTO.Rating,
                Description = reviewDTO.Description,
                ProductID = reviewDTO.ProductID
            };
            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();

            await CountAverageRating(review.ProductID);

            return new Result<int>()
            {
                Status = ResultStatus.Success,
                Data = review.ID
            };
        }

        public async Task<Result<bool>> DeleteAsync(int reviewID)
        {
            Review? review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.ID == reviewID);
            if (review == null)
            {
                return new Result<bool>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Отзыв не найден."
                };
            }

            _dbContext.Reviews.Remove(review);
            await CountAverageRating(review.ProductID);
            await _dbContext.SaveChangesAsync();

            return new Result<bool> 
            {
                Status = ResultStatus.Success,
                Data = true,
                Message = "Отзыв успешно удалён!"
            };
        }

        private async Task CountAverageRating(int productID)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ID == productID);
            if (product != null)
            {
                List<decimal> ratings = await _dbContext.Reviews.Where(r => r.ProductID == productID).Select(r => r.Rating).ToListAsync();

                if (ratings.Count > 0)
                    product.Rating = ratings.Average();
                else
                    product.Rating = 0;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
