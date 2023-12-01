using E_Commerce.Models.Domain;

namespace E_Commerce.Repositories.Interface
{
    public interface ICategoryRespository
    {
        Task<Category> CreateAsync(Category category);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<bool> DeleteAsync(int categoryId);
        Task<Category> UpdateAsync(Category category);


    }
}
