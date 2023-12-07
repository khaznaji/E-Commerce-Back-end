using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Commerce.Repositories.Implementation
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public SubCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubCategory> CreateAsync(SubCategory subcategory)
        {
            if (subcategory == null)
            {
                throw new ArgumentNullException(nameof(subcategory));
            }

            // Check if the referenced category exists
            var existingCategory = await _context.Categories.FindAsync(subcategory.CategoryId);
            if (existingCategory == null)
            {
                throw new InvalidOperationException($"Category with ID {subcategory.CategoryId} not found.");
            }

            // Associate the subcategory with the category
            subcategory.Category = existingCategory;
            subcategory.Archive = false;
            // Add the subcategory to the context
            _context.SubCategory.Add(subcategory);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
                return subcategory;
            }
            catch (Exception ex)
            {
                // Handle the exception according to your needs
                Console.Error.WriteLine($"An error occurred during subcategory creation: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<SubCategory>> GetAllAsync()
        {
            return await _context.SubCategory.ToListAsync();
        }

        public async Task<SubCategory> GetByIdAsync(int subcategoryId)
        {
            return await _context.SubCategory.FindAsync(subcategoryId);
        }

        public async Task<SubCategory> UpdateAsync(SubCategory subcategory)
        {
            _context.Entry(subcategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task<bool> DeleteAsync(int subcategoryId)
        {
            var subcategoryToDelete = await _context.SubCategory.FindAsync(subcategoryId);

            if (subcategoryToDelete == null)
                return false;

            _context.SubCategory.Remove(subcategoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ToggleArchivedAsync(int categoryId)
        {
            var category = await _context.SubCategory.FindAsync(categoryId);

            if (category == null)
            {
                return false; // Catégorie non trouvée
            }

            // Inverser l'état d'archivage
            category.Archive = !category.Archive;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteMultipleAsync(List<int> subcategoryIds)
        {
            foreach (var subcategoryId in subcategoryIds)
            {
                var subcategoryToDelete = await _context.SubCategory.FindAsync(subcategoryId);

                if (subcategoryToDelete != null)
                {
                    _context.SubCategory.Remove(subcategoryToDelete);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAllAsync()
        {
            var subcategoriesToDelete = await _context.SubCategory.ToListAsync();

            if (subcategoriesToDelete == null || !subcategoriesToDelete.Any())
            {
                return false;
            }

            _context.SubCategory.RemoveRange(subcategoriesToDelete);
            await _context.SaveChangesAsync();

            return true;
        }



    }
}
