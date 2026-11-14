using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using static API.DTO.OrderDTO;
using static API.DTO.OrderProductDTO;
using static API.DTO.PostOrderDTO;

namespace API.Services
{
    public class OrderService : IOrderService
    {
        private MarketPlacePR4Context _dbContext;

        public OrderService(MarketPlacePR4Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<OrderDTO>>> GetAllAsync()
        {
            List<OrderDTO> orders = await _dbContext.Orders
                .Select(o => new OrderDTO(
                        o.ID,
                        o.UserID,
                        o.DateOfOrder,
                        o.TotalPrice,
                        o.StatusID,
                        o.OrderProducts.Select(Op => new OrderProductDTO(
                            Op.Product.Name,
                            Op.Product.Price,
                            Op.Quantity)).ToList(),
                        string.IsNullOrWhiteSpace(o.AddressOfDelivery) ? o.PickUpPoint.Address.ToString() : o.AddressOfDelivery.ToString()
                        )
                    ).ToListAsync();
            return new Result<List<OrderDTO>>
            {
                Status = ResultStatus.Success,
                Data = orders
            };
        }

        public async Task<Result<OrderDTO>> GetByIdAsync(int OrderID)
        {
            OrderDTO? order = await _dbContext.Orders.Where(u => u.ID == OrderID)
                .Select(o => new OrderDTO(
                        o.ID,
                        o.UserID,
                        o.DateOfOrder,
                        o.TotalPrice,
                        o.StatusID,
                        o.OrderProducts.Select(Op => new OrderProductDTO(
                            Op.Product.Name,
                            Op.Product.Price,
                            Op.Quantity)).ToList(),
                        string.IsNullOrWhiteSpace(o.AddressOfDelivery) ? o.PickUpPoint.Address.ToString() : o.AddressOfDelivery.ToString()
                    )
                ).FirstOrDefaultAsync();

            if (order == null)
                return new Result<OrderDTO>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Заказ по ID не найден"
                };

            return new Result<OrderDTO>
            {
                Status = ResultStatus.Success,
                Data = order
            };
        }

        public async Task<Result<List<OrderDTO>>> GetByUserIdAsync(int userID)
        {
            List<OrderDTO> orders = await _dbContext.Orders.Where(u => u.UserID == userID)
                .Select(o => new OrderDTO(
                    o.ID,
                    o.UserID,
                    o.DateOfOrder,
                    o.TotalPrice,
                    o.StatusID,
                    o.OrderProducts.Select(Op => new OrderProductDTO(
                            Op.Product.Name,
                            Op.Product.Price,
                            Op.Quantity)).ToList(),
                    string.IsNullOrWhiteSpace(o.AddressOfDelivery) ? o.PickUpPoint.Address.ToString() : o.AddressOfDelivery.ToString()
                    )
                ).ToListAsync();

            return new Result<List<OrderDTO>>
            {
                Status = ResultStatus.Success,
                Data = orders
            };
        }

        public async Task<Result<int>> PostListAsync(int userID, List<PostOrderDTO> Products)
        {
            if (Products == null || Products.Count == 0)
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Заказ пустой"
                };

            if (userID <= 0 || !await _dbContext.Users.AnyAsync(u => u.ID == userID))
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Пользователь не найден"
                };

            if (Products.Any(p => p.Quantity <= 0))
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Количество товара должно быть больше нуля"
                };

            var productIds = Products.Select(p => p.ProductID).ToList();

            if (await _dbContext.Products.CountAsync(p => productIds.Contains(p.ID)) != productIds.Count)
                return new Result<int>
                {
                    Status = ResultStatus.NotFound,
                    Message = "Один или несколько товаров не найдены"
                };

            Order NewOrder = new Order()
            {
                UserID = userID,
                OrderProducts = new List<OrderProduct>(),
                DateOfOrder = DateOnly.FromDateTime(DateTime.Now)
            };
            foreach (var productsInOrder in Products)
                NewOrder.OrderProducts.Add(new OrderProduct
                {
                    ProductID = productsInOrder.ProductID,
                    Quantity = productsInOrder.Quantity,
                });

            _dbContext.Orders.Add(NewOrder);

            await _dbContext.SaveChangesAsync();

            return new Result<int>()
            {
                Status = ResultStatus.Success,
                Data = NewOrder.ID
            };
        }

        public async Task<Result<bool>> UpdateOrderStatusAsync(int orderID, int statusID)
        {
            Order? order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ID == orderID);
            if (order != null)
            {
                if (!await _dbContext.Statuses.AnyAsync(s => s.ID == statusID))
                {
                    return new Result<bool>
                    {
                        Status = ResultStatus.NotFound,
                        Message = "Такой статус не найден."
                    };
                }

                if (order.StatusID == statusID)
                {
                    return new Result<bool>
                    {
                        Status = ResultStatus.InvalidData,
                        Message = "Заказ итак имеет этот статус."
                    };
                }

                order.StatusID = statusID;
                await _dbContext.SaveChangesAsync();

                return new Result<bool>
                {
                    Status = ResultStatus.Success,
                    Data = true,
                    Message = "Статус заказа успешно изменён!"
                };
            }

            return new Result<bool>
            {
                Status = ResultStatus.NotFound,
                Message = "Заказ не найден."
            };
        }
    }
}
