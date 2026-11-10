namespace API.DTO
{
    public class OrderProductDTO
    {
        public record OrderProductDto(ProductDTO Product, int Quantity);
    }
}
