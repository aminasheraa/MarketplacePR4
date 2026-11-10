using API.Models;

namespace API.DTO
{
    public class OrderDTO
    {
        public record OrderDto(int ID, int UserID, DateTime DateOfOrder, decimal TotalPrice, int StatusID, List<OrderProductDTO> Products, string Address);
        
        
    }
}
