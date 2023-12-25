using E_Commerce.Models.Domain;
using E_Commerce.Models.Repositories;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using E_Commerce.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly ApplicationDbContext _dbContext;

        public ProductController(IProductRepository productRepository , ApplicationDbContext dbContext)
        {
            this.productRepository = productRepository;
            _dbContext = dbContext;

        }
        /*[HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProductAsync([FromForm] Product product, [FromForm] List<IFormFile> images)
        {
            try
            {
                // Vérifier si la SubCategory associée au produit existe
                var existingSubCategory = await _dbContext.SubCategory
                    .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

                if (existingSubCategory == null)
                {
                    return BadRequest("SubCategory not found");
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

                // Enregistrer les modifications dans la base de données
                await _dbContext.SaveChangesAsync();

                // Retourner une réponse de succès
                return CreatedAtAction(nameof(GetAllCategories), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }*/
        /*  public async Task<IActionResult> AddProduct([FromForm] Product product)
            {
                try
                {
                    await productRepository.AddProductAsync(product);
                    return Ok(new { id = product.Id, message = "Product added successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error adding product: {ex.Message}");
                }
            }*/
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProductAsync([FromForm] Product product, [FromForm] List<IFormFile> images)
        {
            try
            {
                // Vérifier si la SubCategory associée au produit existe
                var existingSubCategory = await _dbContext.SubCategory
                    .FirstOrDefaultAsync(sc => sc.Id == product.SubCategoryId);

                if (existingSubCategory == null)
                {
                    return BadRequest("SubCategory not found");
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
                await _dbContext.SaveChangesAsync();

                // Configure JsonSerializerOptions to handle reference loops
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    // Add any other options as needed
                };

                // Convert the product object to JSON
                var productJson = JsonSerializer.Serialize(product, jsonSerializerOptions);

                // Deserialize the JSON back to Product to break reference loops
                var productWithoutReferenceLoops = JsonSerializer.Deserialize<Product>(productJson, jsonSerializerOptions);

                // Retourner une réponse de succès
                return CreatedAtAction(nameof(GetAllCategories), new { id = product.Id }, productWithoutReferenceLoops);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] Product product)
        {
            try
            {
                // Ensure the provided product ID matches the route parameter
                if (productId != product.Id)
                {
                    return BadRequest("Product ID in the request body does not match the route parameter.");
                }

                // Call the service method to update the product
                await productRepository.UpdateProductAsync(product);

                return Ok("Product updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                await productRepository.DeleteProductAsync(productId);
                return Ok(); // Retourne un statut 200 OK si la suppression réussit
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllCategories()
        {
            var categories = await productRepository.GetAllAsync();
            return Ok(categories);
        }
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> subcategoryIds)
        {
            if (subcategoryIds == null || !subcategoryIds.Any())
            {
                return BadRequest("Please provide valid subcategory IDs for deletion.");
            }

            try
            {
                var result = await productRepository.DeleteMultipleAsync(subcategoryIds);

                if (result)
                {
                    return Ok("Subcategories deleted successfully.");
                }
                else
                {
                    return NotFound("One or more subcategories were not found for deletion.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var result = await productRepository.DeleteAllAsync();

                if (result)
                {
                    return Ok("All subcategories deleted successfully.");
                }
                else
                {
                    return NotFound("No subcategories found for deletion.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
