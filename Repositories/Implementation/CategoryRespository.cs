﻿using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_Commerce.Repositories.Implementation
{
    public class CategoryRespository : ICategoryRespository

    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CategoryRespository(ApplicationDbContext context)
        { this._context = context; }
        /* public async Task<Category> CreateAsync(Category category)
         {
             _context.Categories.Add(category);
             await _context.SaveChangesAsync();
             return category;
         }*/
        public async Task<Category> CreateAsync(Category category, IFormFile image)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);

            if (existingCategory != null)
            {
                // Category with the same name already exists, return a conflict response
                // You can customize the response based on your application's requirements
                return null;
            }
            // Ensure the image is provided
            if (image != null && image.Length > 0)
            {
                // Generate a unique timestamp for the image file
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                // Get the file extension (e.g., .jpg, .png)
                var fileExtension = Path.GetExtension(image.FileName);

                // Construct the file name with the timestamp and extension
                var fileName = $"{timestamp}{fileExtension}";

                // Combine the file path with the category image directory
                var filePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Categories", fileName);

                // Save the image to the specified pat  h
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Set the ImageUrl property of the category to the saved file path
                category.ImageUrl = $"\\Categories\\{fileName}";
            }
            category.Archive = false;
            // Save the category to the database
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

            // Récupérer le chemin du fichier à partir de la propriété ImageUrl
            var filePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image", categoryToDelete.ImageUrl.TrimStart('\\'));

            // Log des informations pour le débogage
            Console.WriteLine($"Trying to delete file: {filePath}");

            // Supprimer le fichier du disque
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"File deleted successfully: {filePath}");
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<Category> UpdateAsync(Category category, IFormFile newImage)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.Id);

                if (existingCategory == null)
                {
                    // If the category with the given ID is not found, return null or throw an exception
                    return null; // or throw new Exception("Category not found.");
                }

                // Check if a new image is provided
                if (newImage != null && newImage.Length > 0)
                {
                    // Generate a unique timestamp for the new image file
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    // Get the file extension (e.g., .jpg, .png)
                    var fileExtension = Path.GetExtension(newImage.FileName);

                    // Construct the file name with the timestamp and extension
                    var fileName = $"{timestamp}{fileExtension}";

                    // Combine the file path with the category image directory
                    var newFilePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Categories", fileName);

                    // Save the new image to the specified path
                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await newImage.CopyToAsync(stream);
                    }

                    // Delete the old image
                    if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                    {
                        var oldFilePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image", existingCategory.ImageUrl.TrimStart('\\'));

                        // Check if the old image file exists before attempting to delete
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                            Console.WriteLine($"Old file deleted successfully: {oldFilePath}");
                        }
                    }

                    // Set the ImageUrl property of the category to the saved file path
                    existingCategory.ImageUrl = $"\\Categories\\{fileName}";
                }

                // Update other properties of the existing category
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



        public async Task<bool> ToggleArchivedAsync(int categoryId)
        {
            try
            {
                // Retrieve the category
                var category = await _context.Categories.FindAsync(categoryId);

                // Check if the category exists
                if (category == null)
                {
                    return false; // Or throw an exception, depending on your requirements
                }

                // Toggle the archived status of the category
                category.Archive = !category.Archive;

                // Retrieve subcategories associated with the category
                var subcategories = _context.SubCategory.Where(s => s.CategoryId == categoryId);

                // Toggle the archived status of subcategories
                foreach (var subcategory in subcategories)
                {
                    subcategory.Archive = category.Archive;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception according to your needs
                throw new Exception($"Error toggling archived status. Details: {ex.Message}", ex);
            }
        }
        public async Task<Category> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> DeleteMultipleAsync(List<int> subcategoryIds)
        {
            foreach (var subcategoryId in subcategoryIds)
            {
                var subcategoryToDelete = await _context.Categories.FindAsync(subcategoryId);

                if (subcategoryToDelete != null)
                {
                    // Récupérer le chemin du fichier à partir de la propriété ImageUrl
                    var filePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image", subcategoryToDelete.ImageUrl.TrimStart('\\'));

                    // Log des informations pour le débogage
                    Console.WriteLine($"Trying to delete file: {filePath}");

                    // Supprimer le fichier du disque
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            Console.WriteLine($"File deleted successfully: {filePath}");
                        }
                        else
                        {
                            Console.WriteLine("File does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file: {ex.Message}");
                    }

                    _context.Categories.Remove(subcategoryToDelete);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAllAsync()
        {
            var categoriesToDelete = await _context.Categories.ToListAsync();

            if (categoriesToDelete == null || !categoriesToDelete.Any())
            {
                return false;
            }

            foreach (var categoryToDelete in categoriesToDelete)
            {
                // Récupérer le chemin du fichier à partir de la propriété ImageUrl
                var filePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image", categoryToDelete.ImageUrl.TrimStart('\\'));

                // Log des informations pour le débogage
                Console.WriteLine($"Trying to delete file: {filePath}");

                // Supprimer le fichier du disque
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"File deleted successfully: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine("File does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }

            _context.Categories.RemoveRange(categoriesToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
