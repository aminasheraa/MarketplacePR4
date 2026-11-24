using API.DTO;
using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServise _service;

        public CategoryController(ICategoryServise service)
        {
            _service = service;
        }

        // Получение всех категорий
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult();
        }

        // Добавление категории
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Введите название категории");
            }

            var result = await _service.CreateAsync(category);

            return Ok(result);
        }

        // Редактирование категории
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Введите название категории");
            }

            var result = await _service.UpdateAsync(id, category);

            if (result == null)
            {
                return NotFound("Категория не найдена");
            }

            return Ok(result);
        }
    }
}
