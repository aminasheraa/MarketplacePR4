using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Interfaces;

namespace API.Controller
{
    public class ReviewController : ControllerBase
    {
        IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("api/reviewsList")]
        //[Authorize(Roles = "Администратор")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult();
        }

        [HttpGet("api/review/product/{productID}")]
        public async Task<IActionResult> GetByProductID(int productID)
        {
            var result = await _service.GetByProductIDAsync(productID);
            return result.ToActionResult();
        }

        [HttpPost("api/review")]
        public async Task<IActionResult> PostReview(PostReviewDTO postReviewDTO)
        {
            if (!int.TryParse(User.FindFirst("sub")?.Value, out int userID))
                return Unauthorized();

            var result = await _service.PostAsync(userID, postReviewDTO);
            return result.ToActionResult();
        }

        [HttpDelete("api/review/{reviewID}")]
        //[Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteReview(int reviewID)
        {
            var result = await _service.DeleteAsync(reviewID);
            return result.ToActionResult();
        }
    }
}