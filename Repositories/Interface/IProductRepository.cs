using E_Commerce.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Repositories.Interface
{
    public interface IProductRepository
    {
        //  Task AddProductAsync(Product product);
        Task<IActionResult> AddProductAsync([FromForm] Product product, [FromForm] List<IFormFile> images);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<bool> DeleteMultipleAsync(List<int> productIds);
        Task<bool> DeleteAllAsync();
    }
}
