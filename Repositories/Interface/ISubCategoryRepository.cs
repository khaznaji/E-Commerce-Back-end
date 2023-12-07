using E_Commerce.Models.Domain;

namespace E_Commerce.Repositories.Interface
{
    public interface ISubCategoryRepository
    {

        Task<SubCategory> CreateAsync(SubCategory subcategory);
        Task<IEnumerable<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(int subcategoryId);
        Task<SubCategory> UpdateAsync(SubCategory subcategory);
        Task<bool> DeleteAsync(int subcategoryId);
        Task<bool> ToggleArchivedAsync(int categoryId);
        Task<bool> DeleteMultipleAsync(List<int> subcategoryIds);
        Task<bool> DeleteAllAsync();


    }
}
