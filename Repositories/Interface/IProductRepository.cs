using E_Commerce.Models.Domain;

namespace E_Commerce.Repositories.Interface
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<bool> DeleteMultipleAsync(List<int> productIds);
        Task<bool> DeleteAllAsync();
    }
}
