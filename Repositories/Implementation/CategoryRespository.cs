using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories.Implementation
{
    public class CategoryRespository : ICategoryRespository

    {
        private readonly ApplicationDbContext _context;
        public CategoryRespository(ApplicationDbContext context)
        { this._context = context; }
        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<bool> DeleteAsync(int categoryId)
        {
            var categoryToDelete = await _context.Categories.FindAsync(categoryId);

            if (categoryToDelete == null)
                return false;

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<Category> UpdateAsync(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.Id);

                if (existingCategory == null)
                {
                    // If the category with the given ID is not found, return null or throw an exception
                    return null; // or throw new Exception("Category not found.");
                }

                // Update the properties of the existing category
                existingCategory.Name = category.Name;

                _context.Entry(existingCategory).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return existingCategory;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.Error.WriteLine(ex);
                // Propagate the exception or handle it based on your requirements
                throw;
            }
        }
    }
}

