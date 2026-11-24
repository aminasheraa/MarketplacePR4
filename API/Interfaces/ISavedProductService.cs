using API.DTO;


namespace API.Interfaces
{
    public enum SavedProductStatus { Ok, ProductNotFound, AlreadyExists, NotInSavedProduct}
    public interface ISavedProductService
    {
        Task<List<SavedProductDto>> GetAsync(int userId);
        Task<SavedProductStatus> AddAsync(int userId, int productId);
        Task<SavedProductStatus> RemoveAsync(int userId, int productId);
    }
}
