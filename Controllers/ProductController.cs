using E_Commerce.Models.Domain;
using E_Commerce.Models.Repositories;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using E_Commerce.Repositories.Implementation;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                await productRepository.AddProductAsync(product);
                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding product: {ex.Message}");
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
