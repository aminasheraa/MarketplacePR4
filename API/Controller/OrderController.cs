using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controller
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        IOrderService _service;

        public OrderController(IOrderService service)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("My")]
        public async Task<IActionResult> GetAllByUser()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized();

            var result = await _service.GetByUserIdAsync(userId);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateList(
            [FromBody] List<PostOrderDTO> Products,
            [FromQuery] int typeOfReceivingId,
            [FromQuery] int? pickUpPointId,
            [FromQuery] string address)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized();

            var result = await _service.PostListAsync(
                userId,
                Products,
                typeOfReceivingId,
                pickUpPointId,
                address);

            return result.ToActionResult();
        }

        [HttpGet("statuses")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> GetAllStatuses()
        {
            var result = await _service.GetAllStatusesAsync();
            return result.ToActionResult();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO statusDTO)
        {
            var result = await _service.UpdateOrderStatusAsync(id, statusDTO.StatusID);
            return result.ToActionResult();
        }

        [HttpGet("ReceivingTypes")]
        public async Task<IActionResult> GetReceivingTypes()
        {
            var result = await _service.GetReceivingTypesAsync();
            return Ok(result);
        }

        [HttpGet("PickUpPoints")]
        public async Task<IActionResult> GetPickUpPoints()
        {
            var result = await _service.GetPickUpPointsAsync();
            return Ok(result);
        }
    }
}
