using E_Commerce.Models.Domain;

namespace E_Commerce.Repositories.Interface
{
    public interface ICategoryRespository
    {
        Task<Category> CreateAsync(Category category, IFormFile image);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<bool> DeleteAsync(int categoryId);
        Task<Category> UpdateAsync(Category category, IFormFile image);
         Task<bool> ToggleArchivedAsync(int categoryId);
        Task<Category> GetByNameAsync(string name);
        Task<bool> DeleteMultipleAsync(List<int> subcategoryIds);
        Task<bool> DeleteAllAsync();


    }
}
