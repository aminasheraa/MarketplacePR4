using API.DTO;
using API.Intarfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class SavedProductService(MarketPlacePR4Context db) : ISavedProductService
    {
        public Task<List<SavedProductDto>> GetAsync(int userId) => db.SavedProducts.Where(sp => sp.Saved.UserID == userId)
            .OrderBy(sp => sp.ID).Select(sp => new SavedProductDto(sp.ProductID, sp.Product.Name, sp.Product.Price, 
                sp.Product.Image, sp.Product.Rating, sp.Product.IsAvailable, sp.Product.StockQuantity)).ToListAsync();

        public async Task<SavedProductStatus> AddAsync(int userId, int productId)
        {
            if (!await db.Products.AnyAsync(p => p.ID == productId)) return SavedProductStatus.ProductNotFound;

            var alreadyAdded = await db.SavedProducts.AnyAsync(sp => sp.Saved.UserID == userId 
                && sp.ProductID == productId);

            if (alreadyAdded) return SavedProductStatus.AlreadyExists;

            var list = await GetOnCreateListAsync(userId);
            db.SavedProducts.Add(new SavedProduct { SavedID = list.ID, ProductID = productId });
            await db.SaveChangesAsync();
            return SavedProductStatus.Ok;
        }

        public async Task<SavedProductStatus> RemoveAsync(int userId, int productId)
        {
            var item = await db.SavedProducts.FirstOrDefaultAsync(sp => sp.Saved.UserID == userId 
                && sp.ProductID == productId);

            if (item is null) 
                return SavedProductStatus.NotInSavedProduct;

            db.SavedProducts.Remove(item);
            await db.SaveChangesAsync();
            return SavedProductStatus.Ok;
        }

        private async Task<Saved> GetOnCreateListAsync(int userId)
        {
            var list = await db.Saveds.FirstOrDefaultAsync(sp => sp.UserID == userId);
            if (list is null)
            {
                list = new Saved { UserID = userId };
                db.Saveds.Add(list);
                await db.SaveChangesAsync();
            }
            return list;
        }
    }
}
