using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Interfaces;

namespace API.Controller
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("List")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult();
        }

        [HttpGet("{reviewID}")]
        public async Task<IActionResult> GetByID(int reviewID)
        {
            var result = await _service.GetByIDAsync(reviewID);
            return result.ToActionResult();
        }

        [HttpGet("product/{productID}")]
        public async Task<IActionResult> GetByProductID(int productID)
        {
            var result = await _service.GetByProductIDAsync(productID);
            return result.ToActionResult();
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostReview(PostReviewDTO postReviewDTO)
        {
            if (!int.TryParse(User.FindFirst("sub")?.Value, out int userID))
                return Unauthorized();

            var result = await _service.PostAsync(userID, postReviewDTO);
            return result.ToActionResult();
        }

        [HttpDelete("del/{reviewID}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteReview(int reviewID)
        {
            var result = await _service.DeleteAsync(reviewID);
            return result.ToActionResult();
        }
    }
}