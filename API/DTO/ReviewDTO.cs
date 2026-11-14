using API.DTO;
using API.Models;

namespace API.DTO
{
    public record ReviewDTO(int ID, DateOnly Date, decimal Rating, string Description, string UserName, string ProductName);
}
