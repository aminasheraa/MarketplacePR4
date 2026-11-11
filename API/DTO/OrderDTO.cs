using API.Models;

namespace API.DTO
{

        public record OrderDTO(int ID, int UserID, DateOnly DateOfOrder, decimal TotalPrice, int StatusID, List<OrderProductDTO> Products, string Adress);
        
        
    
}
