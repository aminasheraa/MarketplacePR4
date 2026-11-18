using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Interfaces;

namespace API.Controller
{
    public class OrderController : ControllerBase
    {
        IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Менеджер")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Менеджер,Администратор")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("My")]
        public async Task<IActionResult> GetAllByUser()
        {
            if (!int.TryParse(User.FindFirst("sub")?.Value, out int userId))
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
            if (!int.TryParse(User.FindFirst("sub")?.Value, out int userId))
                return Unauthorized();

            var result = await _service.PostListAsync(
                userId,
                Products,
                typeOfReceivingId,
                pickUpPointId,
                address);

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
