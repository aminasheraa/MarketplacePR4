namespace API.DTO
{
    public record CartItemDTO(int ID, int ProductID, int Quantity, ProductDTO Product);
}
