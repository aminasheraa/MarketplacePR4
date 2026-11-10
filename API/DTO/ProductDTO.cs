namespace API.DTO
{
    public class ProductDTO
    {
        public record ProductDto(int ID, string Name, int CategoryID, string Description, decimal Price, string Image, decimal Rating, int PackageCount, bool IsAvailable, int StockQuantity);

    }
}
