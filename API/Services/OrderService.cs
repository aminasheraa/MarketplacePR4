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
                            Op.Product.Image,
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
                            Op.Product.Image,
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
                            Op.Product.Image,
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

        public async Task<Result<int>> PostListAsync(
    int userID,
    List<PostOrderDTO> Products,
    int typeOfReceivingId,
    int? pickUpPointId,
    string address)
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

            if (!await _dbContext.Set<TypeOfReceiving>()
                .AnyAsync(t => t.ID == typeOfReceivingId))
                return new Result<int>
                {
                    Status = ResultStatus.InvalidData,
                    Message = "Способ получения не найден"
                };

            if (typeOfReceivingId == 1)
            {
                if (pickUpPointId == null ||
                    !await _dbContext.Set<PickUpPoint>()
                        .AnyAsync(p => p.ID == pickUpPointId))
                    return new Result<int>
                    {
                        Status = ResultStatus.InvalidData,
                        Message = "Выберите пункт самовывоза"
                    };

                address = null;
            }
            else if (typeOfReceivingId == 2)
            {
                if (string.IsNullOrWhiteSpace(address))
                    return new Result<int>
                    {
                        Status = ResultStatus.InvalidData,
                        Message = "Введите адрес доставки"
                    };

                pickUpPointId = null;
            }

            var newOrder = new Order()
            {
                UserID = userID,
                DateOfOrder = DateOnly.FromDateTime(DateTime.Today),
                TypeOfReceivingID = typeOfReceivingId,
                PickUpPointID = pickUpPointId,
                AddressOfDelivery = address,
                StatusID = 1,
                OrderProducts = new List<OrderProduct>()
            };

            decimal totalPrice = 0;

            foreach (var item in Products)
            {
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.ID == item.ProductID);

                if (product == null)
                    return new Result<int>
                    {
                        Status = ResultStatus.NotFound,
                        Message = "Один или несколько товаров не найдены"
                    };

                totalPrice += product.Price * item.Quantity;

                newOrder.OrderProducts.Add(new OrderProduct
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity
                });
            }

            newOrder.TotalPrice = totalPrice;

            _dbContext.Orders.Add(newOrder);

            await _dbContext.SaveChangesAsync();

            return new Result<int>
            {
                Status = ResultStatus.Success,
                Data = newOrder.ID
            };
        }

        public async Task<List<TypeOfReceiving>> GetReceivingTypesAsync()
        {
            return await _dbContext.Set<TypeOfReceiving>()
                .Select(t => new TypeOfReceiving
                {
                    ID = t.ID,
                    Name = t.Name
                })
                .ToListAsync();
        }

        public async Task<List<PickUpPoint>> GetPickUpPointsAsync()
        {
            return await _dbContext.Set<PickUpPoint>()
                .Select(p => new PickUpPoint
                {
                    ID = p.ID,
                    Address = p.Address
                })
                .ToListAsync();
        }

    }
}
