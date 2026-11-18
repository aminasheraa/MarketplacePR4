namespace API.DTO
{
    public record ProductDTO(int ID, string Name, int CategoryID, string Description, decimal Price, string Image, decimal Rating, int PackageCount, bool IsAvailable, int StockQuantity);
}
