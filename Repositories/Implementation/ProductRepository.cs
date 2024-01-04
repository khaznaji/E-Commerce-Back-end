using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Data;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;


        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
        /* public async Task AddProductAsync(Product product)                      
         {
             // Ajouter des validations supplémentaires si nécessaire

             // Vérifier si la SubCategory associée au produit existe
             var existingSubCategory = await _dbContext.SubCategory
                 .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

             if (existingSubCategory == null)
             {
                 // La SubCategory n'existe pas, vous pouvez gérer cela en fonction de vos besoins
                 // Vous pourriez lever une exception, créer la SubCategory, etc.
                 throw new InvalidOperationException("SubCategory not found");
             }

             // Associer la SubCategory au produit
             product.SubCategory = existingSubCategory;

             // Ajouter le produit à la base de données de manière asynchrone
             await _dbContext.Products.AddAsync(product);

             // Enregistrer les modifications dans la base de données
             await _dbContext.SaveChangesAsync();

             // Créer le dossier pour stocker les images du produit
             var productFolder = $"{product.Id}_{product.Name}";
             var path = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce Image\\Products", productFolder);

             // Vérifier si le dossier existe, sinon le créer
             if (!Directory.Exists(path))
             {
                 Directory.CreateDirectory(path);
             }

             // Créer une nouvelle liste pour stocker les URLs des images mises à jour
             var updatedImageUrls = new List<string>();

             // Ajouter les URLs des images associées au produit
             if (product.ImageUrls != null && product.ImageUrls.Any())
             {
                 foreach (var imageUrl in product.ImageUrls)
                 {
                     // Créer un nom de fichier unique en ajoutant un timestamp
                     var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                     var fileName = $"{timestamp}_{Path.GetFileName(imageUrl)}";

                     // Copier l'image vers le dossier du produit
                     var destinationPath = Path.Combine(path, fileName);
                     File.Copy(imageUrl, destinationPath, true);

                     // Stocker le chemin relatif de l'image dans la liste mise à jour
                     updatedImageUrls.Add(Path.Combine(productFolder, fileName));
                 }

                 // Assigner la liste mise à jour à product.ImageUrls
                 product.ImageUrls = updatedImageUrls;
                 await _dbContext.Products.AddAsync(product);

                 // Enregistrer les modifications dans la base de données
                 await _dbContext.SaveChangesAsync();

             }
         }*/
        public async Task UpdateProductNewAsync(int productId, [FromForm] Product product, [FromForm] List<IFormFile> images)
        {
            // Ajouter des validations supplémentaires si nécessaire

            // Vérifier si la SubCategory associée au produit existe
            var existingSubCategory = await _dbContext.SubCategory
                .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

            if (existingSubCategory == null)
            {
                // La SubCategory n'existe pas, vous pouvez gérer cela en fonction de vos besoins
                // Vous pourriez lever une exception, créer la SubCategory, etc.
                throw new InvalidOperationException("SubCategory not found");
            }

            // Vérifier si le produit existe déjà dans la base de données
            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id);
            if (!string.Equals(existingProduct.Name, product.Name, StringComparison.OrdinalIgnoreCase))
            {
                // Renommer le dossier du produit avec le nouveau nom
                await RenameProductFolder(existingProduct, product.Name);
            }
            if (existingProduct == null)
            {
                // Le produit n'existe pas, lever une exception
                throw new InvalidOperationException("Product not found for update");
            }

            var oldProductFolder = $"{existingProduct.Id}_{existingProduct.Name}";
            var oldProductName = existingProduct.Name; // Extraire l'ancien nom du produit
            var oldPath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products");

            // Comparer et mettre à jour les propriétés du produit
            existingProduct.Name = product.Name;
            existingProduct.OriginalPrice = product.OriginalPrice;
            existingProduct.DiscountedPrice = product.DiscountedPrice;
            existingProduct.Description = product.Description;
            existingProduct.Stock = product.Stock;
            existingProduct.Color = product.Color;
            existingProduct.Size = product.Size;
            existingProduct.Material = product.Material;
            existingProduct.Composition = product.Composition;
            existingProduct.Col = product.Col;
            existingProduct.Promo = product.Promo;
           // existingProduct.Onsale = product.Onsale;
            existingProduct.Date = product.Date;

            // Vérifier si le nom du produit a changé
            if (existingProduct.Name != product.Name)
            {
                // Renommer le dossier du produit avec le nouveau nom
                var newProductFolder = $"{existingProduct.Id}_{product.Name}";
                var newPath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products", newProductFolder);

                try
                {
                    // Supprimer le dossier existant s'il existe
                    if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                }
                catch (Exception ex)
                {
                    // Ajoutez une gestion d'erreur appropriée ici
                    Console.WriteLine($"Erreur lors du déplacement du dossier : {ex.Message}");
                    throw;
                }

                // Mettre à jour le chemin avec le nouveau chemin
                oldPath = newPath;
            }
            var updatedImageUrls = new List<string>();

            if (images != null && images.Any())
            {
                var destinationFolder = Path.Combine(oldPath, $"{existingProduct.Id}_{existingProduct.Name}");

                // Récupérer la liste des anciennes images associées au produit
                var oldImageUrls = existingProduct.ImageUrls.ToList();

                // Supprimer les images qui ne sont plus associées au produit
                foreach (var oldImageUrl in oldImageUrls)
                {
                    if (!images.Any(newImage => IsSameImage(oldImageUrl, newImage)))
                    {
                        var oldImagePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products", oldImageUrl);

                        try
                        {
                            File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            // Ajoutez une gestion d'erreur appropriée ici
                            Console.WriteLine($"Erreur lors de la suppression de l'image existante : {ex.Message}");
                            throw;
                        }

                        // Retirer l'ancienne image de la liste
                        existingProduct.ImageUrls.Remove(oldImageUrl);
                    }
                }

                // Copier les nouvelles images
                foreach (var image in images)
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileName = $"{timestamp}_{Path.GetFileName(image.FileName)}";

                    // Mettre à jour le chemin avec le nouveau chemin (ajout du sous-dossier)
                    var destinationPath = Path.Combine(destinationFolder, fileName);

                    try
                    {
                        using (var stream = new FileStream(destinationPath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ajoutez une gestion d'erreur appropriée ici
                        Console.WriteLine($"Erreur lors de la copie de l'image : {ex.Message}");
                        throw;
                    }

                    updatedImageUrls.Add(Path.Combine(existingProduct.Id + "_" + existingProduct.Name, fileName));
                }

                // Supprimer les images qui ne sont plus associées au produit
                var removedImages = existingProduct.ImageUrls
                    .Where(imageUrl => !updatedImageUrls.Contains(imageUrl))
                    .ToList();

                foreach (var removedImage in removedImages)
                {
                    var removedImagePath = Path.Combine(oldPath, Path.GetFileName(removedImage));

                    try
                    {
                        File.Delete(removedImagePath);
                    }
                    catch (Exception ex)
                    {
                        // Ajoutez une gestion d'erreur appropriée ici
                        Console.WriteLine($"Erreur lors de la suppression de l'image : {ex.Message}");
                        throw;
                    }
                }

                existingProduct.ImageUrls = updatedImageUrls;
            }

            await _dbContext.SaveChangesAsync();
        }

        // Méthode utilitaire pour vérifier si deux images sont identiques
        private bool IsSameImage(string imageUrl, IFormFile newImage)
        {
            // Vous pouvez implémenter une logique pour comparer les images, par exemple en comparant les noms de fichiers, les tailles, etc.
            return Path.GetFileName(imageUrl) == Path.GetFileName(newImage.FileName);
        }

        public async Task<IActionResult> AddProductAsync([FromForm] Product product, [FromForm] List<IFormFile> images)
    {
        // Vérifier si la SubCategory associée au produit existe
        var existingSubCategory = await _dbContext.SubCategory
            .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

        if (existingSubCategory == null)
        {
            // La SubCategory n'existe pas, vous pouvez gérer cela en fonction de vos besoins
            // Vous pourriez lever une exception, créer la SubCategory, etc.
            throw new InvalidOperationException("SubCategory not found");
        }

        // Associer la SubCategory au produit
        product.SubCategory = existingSubCategory;

        // Ajouter le produit à la base de données de manière asynchrone
        await _dbContext.Products.AddAsync(product);

        // Enregistrer les modifications dans la base de données
        await _dbContext.SaveChangesAsync();

        // Créer le dossier pour stocker les images du produit
        var productFolder = $"{product.Id}_{product.Name}";
        var path = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products", productFolder);

        // Vérifier si le dossier existe, sinon le créer
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var updatedImageUrls = new List<string>();

        // Sauvegarder chaque image téléchargée
        foreach (var formFile in images)
        {
            if (formFile.Length > 0)
            {
                // Créer un nom de fichier unique avec un timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"{timestamp}_{formFile.FileName}";

                var filePath = Path.Combine(path, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                updatedImageUrls.Add(Path.Combine(productFolder, fileName));
            }
        }

        // Mettre à jour les URL des images dans le produit
        product.ImageUrls = updatedImageUrls;

        // Ajouter le produit à la base de données avec les images mises à jour
        await _dbContext.Products.AddAsync(product);

        // Enregistrer les modifications dans la base de données
        await _dbContext.SaveChangesAsync();

            // Retourner une réponse de succès
            return null; 
    }

    private async Task RenameProductFolder(Product existingProduct, string newProductName)
        {
            var oldProductFolder = $"{existingProduct.Id}_{existingProduct.Name}";
            var newProductFolder = $"{existingProduct.Id}_{newProductName}";
            var basePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products");

            var oldPath = Path.Combine(basePath, oldProductFolder);
            var newPath = Path.Combine(basePath, newProductFolder);

            try
            {
                if (Directory.Exists(oldPath))
                {
                    Directory.Move(oldPath, newPath);
                    existingProduct.ImageUrls = existingProduct.ImageUrls
                        .Select(url => url.Replace(oldProductFolder, newProductFolder))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du déplacement du dossier : {ex.Message}");
                throw;
            }
        }
            public async Task UpdateProductAsync(Product product)
        {
            // Ajouter des validations supplémentaires si nécessaire

            // Vérifier si la SubCategory associée au produit existe
            var existingSubCategory = await _dbContext.SubCategory
                .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

            if (existingSubCategory == null)
            {
                // La SubCategory n'existe pas, vous pouvez gérer cela en fonction de vos besoins
                // Vous pourriez lever une exception, créer la SubCategory, etc.
                throw new InvalidOperationException("SubCategory not found");
            }

            // Vérifier si le produit existe déjà dans la base de données
            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id);
            if (!string.Equals(existingProduct.Name, product.Name, StringComparison.OrdinalIgnoreCase))
            {
                // Renommer le dossier du produit avec le nouveau nom
                await RenameProductFolder(existingProduct, product.Name);
            }
            if (existingProduct == null)
            {
                // Le produit n'existe pas, lever une exception
                throw new InvalidOperationException("Product not found for update");
            }

            var oldProductFolder = $"{existingProduct.Id}_{existingProduct.Name}";
            var oldProductName = existingProduct.Name; // Extraire l'ancien nom du produit
            var oldPath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products");

            // Comparer et mettre à jour les propriétés du produit
            existingProduct.Name = product.Name;
            existingProduct.OriginalPrice = product.OriginalPrice;
            existingProduct.DiscountedPrice = product.DiscountedPrice;
            existingProduct.Description = product.Description;
            existingProduct.Stock = product.Stock;
            existingProduct.Color = product.Color;
            existingProduct.Size = product.Size;
            existingProduct.Material = product.Material;
            existingProduct.Composition = product.Composition;
            existingProduct.Col = product.Col;
            existingProduct.Promo = product.Promo;
          //  existingProduct.Onsale = product.Onsale;
            existingProduct.Date = product.Date;

            // Vérifier si le nom du produit a changé
            if (existingProduct.Name != product.Name)
            {
                // Renommer le dossier du produit avec le nouveau nom
                var newProductFolder = $"{existingProduct.Id}_{product.Name}";
                var newPath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products", newProductFolder);

                try
                {
                    // Supprimer le dossier existant s'il existe
                    if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                }
                catch (Exception ex)
                {
                    // Ajoutez une gestion d'erreur appropriée ici
                    Console.WriteLine($"Erreur lors du déplacement du dossier : {ex.Message}");
                    throw;
                }

                // Mettre à jour le chemin avec le nouveau chemin
                oldPath = newPath;
            }

            var updatedImageUrls = new List<string>();

            if (product.ImageUrls != null && product.ImageUrls.Any())
            {
                var destinationFolder = Path.Combine(oldPath, $"{existingProduct.Id}_{existingProduct.Name}");

                // Récupérer la liste des anciennes images associées au produit
                var oldImageUrls = existingProduct.ImageUrls.ToList();

                // Supprimer les images qui ne sont plus associées au produit
                foreach (var oldImageUrl in oldImageUrls)
                {
                    if (!product.ImageUrls.Contains(oldImageUrl))
                    {
                        var oldImagePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products", oldImageUrl);

                        try
                        {
                            File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            // Ajoutez une gestion d'erreur appropriée ici
                            Console.WriteLine($"Erreur lors de la suppression de l'image existante : {ex.Message}");
                            throw;
                        }

                        // Retirer l'ancienne image de la liste
                        existingProduct.ImageUrls.Remove(oldImageUrl);
                    }
                }

                // Copier les nouvelles images
                foreach (var imageUrl in product.ImageUrls)
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileName = $"{timestamp}_{Path.GetFileName(imageUrl)}";

                    // Mettre à jour le chemin avec le nouveau chemin (ajout du sous-dossier)
                    var destinationPath = Path.Combine(destinationFolder, fileName);

                    try
                    {
                        File.Copy(imageUrl, destinationPath, true);
                    }
                    catch (Exception ex)
                    {
                        // Ajoutez une gestion d'erreur appropriée ici
                        Console.WriteLine($"Erreur lors de la copie de l'image : {ex.Message}");
                        throw;
                    }

                    updatedImageUrls.Add(Path.Combine(existingProduct.Id + "_" + existingProduct.Name, fileName));
                }

                var removedImages = existingProduct.ImageUrls
                    .Where(imageUrl => !updatedImageUrls.Contains(imageUrl))
                    .ToList();

                foreach (var removedImage in removedImages)
                {
                    var removedImagePath = Path.Combine(oldPath, Path.GetFileName(removedImage));

                    try
                    {
                        File.Delete(removedImagePath);
                    }
                    catch (Exception ex)
                    {
                        // Ajoutez une gestion d'erreur appropriée ici
                        Console.WriteLine($"Erreur lors de la suppression de l'image : {ex.Message}");
                        throw;
                    }
                }

                existingProduct.ImageUrls = updatedImageUrls;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (existingProduct == null)
            {
                throw new InvalidOperationException("Product not found for delete");
            }

            // Supprimer le dossier associé au produit
            await DeleteProductFolder(existingProduct);

            // Supprimer le produit de la base de données
            _dbContext.Products.Remove(existingProduct);
            await _dbContext.SaveChangesAsync();
        }

        private async Task DeleteProductFolder(Product product)
        {
            var productFolder = $"{product.Id}_{product.Name}";
            var basePath = Path.Combine("C:\\Users\\DELL\\Desktop\\E-Commerce Front\\E-commerce\\src\\assets\\E-Commerce-Image\\Products");
            var productPath = Path.Combine(basePath, productFolder);

            try
            {
                if (Directory.Exists(productPath))
                {
                    Directory.Delete(productPath, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du dossier du produit : {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }
        public async Task<bool> DeleteMultipleAsync(List<int> productIds)
        {
            foreach (var productId in productIds)
            {
                var productToDelete = await _dbContext.Products.FindAsync(productId);

                if (productToDelete != null)
                {
                    await DeleteProductFolder(productToDelete); // Appeler la méthode pour supprimer le dossier
                    _dbContext.Products.Remove(productToDelete);
                }
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAllAsync()
        {
            var productsToDelete = await _dbContext.Products.ToListAsync();

            if (productsToDelete == null || !productsToDelete.Any())
            {
                return false;
            }

            foreach (var product in productsToDelete)
            {
                await DeleteProductFolder(product);
            }

            _dbContext.Products.RemoveRange(productsToDelete);
            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}
