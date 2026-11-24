using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Interfaces;


namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // Получение всех товаров
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult();
        }

        // Получение товара по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetAsync(id);
            return result.ToActionResult();
        }

        // Добавление товара
        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create([FromBody] ProductDTO product)
        {
            var result = await _service.PostAsync(product);
            return result.ToActionResult();
        }

        // Изменение товара
        [HttpPut("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO product)
        {
            var result = await _service.PutAsync(id, product);
            return result.ToActionResult();
        }
    }
}

