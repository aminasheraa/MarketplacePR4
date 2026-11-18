using API.Intarfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controller
{
    [ApiController]
    [Route("savedproduct")]
    [Authorize]
    public class SavedProductController(ISavedProductService savedProducts) : ControllerBase
    {
        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await savedProducts.GetAsync(UserId));

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            var status = await savedProducts.AddAsync(UserId, productId);
            return status switch
            {
                SavedProductStatus.Ok => Ok(),
                SavedProductStatus.ProductNotFound => NotFound("Продукт не найден"),
                SavedProductStatus.AlreadyExists => Conflict("Товар уже в избранном"),
                _ => Problem()
            };
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var status = await savedProducts.RemoveAsync(UserId, productId);
            return status switch
            {
                SavedProductStatus.Ok => Ok(),
                SavedProductStatus.NotInSavedProduct => NotFound("Товара нет в избранном"),
                _ => Problem()
            };
        }
    }
}
