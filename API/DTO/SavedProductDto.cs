namespace API.DTO
{
    public record SavedProductDto(int ProductId, string Name, decimal Price,
        string Image, decimal Rating, bool IsAvailable, int StockQuantity);
}
