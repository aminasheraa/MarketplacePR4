
using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controller
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet("My")]
        public async Task<IActionResult> GetMyCart()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userID))
                return Unauthorized();

            var result = await _service.GetByUserIdAsync(userID);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PostOrderDTO product)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userID))
                return Unauthorized();

            var result = await _service.AddAsync(userID, product);

            return result.ToActionResult();
        }

        [HttpPut("{id}/{quantity}")]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userID))
                return Unauthorized();

            var result = await _service.UpdateQuantityAsync(userID, id, quantity);

            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userID))
                return Unauthorized();

            var result = await _service.DeleteAsync(userID, id);

            return result.ToActionResult();
        }
    }
}
