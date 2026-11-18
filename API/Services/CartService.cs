
using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CartService : ICartService
    {
        private readonly MarketPlacePR4Context _dbContext;

        public CartService(MarketPlacePR4Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<CartItemDTO>>> GetByUserIdAsync(int userID)
        {
            if (userID <= 0)
                return new Result<List<CartItemDTO>>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Некорректный ID пользователя"
                };

            var cart = await _dbContext.Carts
                .FirstOrDefaultAsync(c => c.UserID == userID);

            if (cart == null)
                return new Result<List<CartItemDTO>>
                {
                    Status = ResultStatus.Success,
                    Data = new List<CartItemDTO>()
                };

            List<CartItemDTO> products = await _dbContext.ProductCarts
                .Where(pc => pc.CartID == cart.ID)
                .Select(pc => new CartItemDTO(
                    pc.ID,
                    pc.ProductID,
                    pc.Quantity,
                    new ProductDTO(
                        pc.Product.ID,
                        pc.Product.Name,
                        pc.Product.CategoryID,
                        pc.Product.Description,
                        pc.Product.Price,
                        pc.Product.Image,
                        pc.Product.Rating,
                        pc.Product.PackageCount ?? 0,
                        pc.Product.IsAvailable,
                        pc.Product.StockQuantity
                    )
                ))
                .ToListAsync();

            return new Result<List<CartItemDTO>>
            {
                Status = ResultStatus.Success,
                Data = products
            };
        }

        public async Task<Result<int>> AddAsync(int userID, PostOrderDTO product)
        {
            if (product == null || product.ProductID <= 0 || product.Quantity <= 0)
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Некорректные данные товара"
                };

            if (!await _dbContext.Users.AnyAsync(u => u.ID == userID))
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Пользователь не найден"
                };

            var selectedProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ID == product.ProductID);

            if (selectedProduct == null)
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар не найден"
                };

            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserID == userID);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userID,
                    TotalPrice = 0
                };

                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            var cartProduct = await _dbContext.ProductCarts.FirstOrDefaultAsync(pc => pc.CartID == cart.ID && pc.ProductID == product.ProductID);

            if (cartProduct == null)
            {
                cartProduct = new ProductCart
                {
                    CartID = cart.ID,
                    ProductID = product.ProductID,
                    Quantity = product.Quantity
                };

                _dbContext.ProductCarts.Add(cartProduct);
            }
            else
            {
                cartProduct.Quantity += product.Quantity;
            }

            await _dbContext.SaveChangesAsync();

            await RecalculateTotalPriceAsync(cart.ID);

            return new Result<int>
            {
                Status = ResultStatus.Success,
                Data = cartProduct.ID
            };
        }

        public async Task<Result<bool>> UpdateQuantityAsync(
            int userID, int productCartID, int quantity)
        {
            if (quantity <= 0)
                return new Result<bool>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Количество должно быть больше нуля"
                };

            var cartProduct = await _dbContext.ProductCarts.Include(pc => pc.Cart).FirstOrDefaultAsync(pc => pc.ID == productCartID && pc.Cart.UserID == userID);

            if (cartProduct == null)
                return new Result<bool>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар в корзине не найден"
                };

            cartProduct.Quantity = quantity;
            await _dbContext.SaveChangesAsync();
            await RecalculateTotalPriceAsync(cartProduct.CartID);

            return new Result<bool>
            {
                Status = ResultStatus.Success,
                Data = true
            };
        }

        public async Task<Result<bool>> DeleteAsync(int userID, int productCartID)
        {
            var cartProduct = await _dbContext.ProductCarts.Include(pc => pc.Cart).FirstOrDefaultAsync(pc => pc.ID == productCartID && pc.Cart.UserID == userID);

            if (cartProduct == null)
                return new Result<bool>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Товар в корзине не найден"
                };

            int cartID = cartProduct.CartID;

            _dbContext.ProductCarts.Remove(cartProduct);

            await _dbContext.SaveChangesAsync();

            await RecalculateTotalPriceAsync(cartID);

            return new Result<bool>
            {
                Status = ResultStatus.Success,
                Data = true
            };
        }

        private async Task RecalculateTotalPriceAsync(int cartID)
        {
            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.ID == cartID);

            if (cart == null)
                return;

            cart.TotalPrice = await _dbContext.ProductCarts.Where(pc => pc.CartID == cartID).SumAsync(pc => pc.Product.Price * pc.Quantity);

            await _dbContext.SaveChangesAsync();
        }
    }
}
