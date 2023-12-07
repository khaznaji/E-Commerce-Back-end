using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Implementation;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryRepository subcategoryRepository;

        public SubCategoryController(ISubCategoryRepository subcategoryRepository)
        {
           this.subcategoryRepository = subcategoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubcategory([FromForm] SubCategory subcategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        // Ajoutez d'autres options si nécessaire
                    };

                    var createdSubcategory = await subcategoryRepository.CreateAsync(subcategory);

                    // Sérialiser l'objet créé en JSON en utilisant les options
                    var jsonResult = JsonSerializer.Serialize(createdSubcategory, options);

                    return Ok(jsonResult);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                // Vous pouvez également renvoyer une réponse HTTP 500 avec des détails sur l'erreur
                return StatusCode(500, $"An error occurred during subcategory creation. Details: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSubcategories()
        {
            var subcategories = await subcategoryRepository.GetAllAsync();
            return Ok(subcategories);
        }

        [HttpGet("{subcategoryId}")]
        public async Task<IActionResult> GetSubcategoryById(int subcategoryId)
        {
            var subcategory = await subcategoryRepository.GetByIdAsync(subcategoryId);

            if (subcategory == null)
                return NotFound($"Subcategory with ID {subcategoryId} not found.");
            return Ok(subcategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubcategory(int id, [FromForm] SubCategory subcategory)
        {
            if (id != subcategory.Id)
            {
                return BadRequest("Mismatched IDs in the request.");
            }
            try
            {
                var updatedSubcategory = await subcategoryRepository.UpdateAsync(subcategory);

                if (updatedSubcategory == null)
                    return NotFound($"Subcategory with ID {subcategory.Id} not found.");

                return Ok(updatedSubcategory);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, $"An error occurred during subcategory update. Details: {ex.Message}");
            }
        }

        [HttpDelete("{subcategoryId}")]
        public async Task<IActionResult> DeleteSubcategory(int subcategoryId)
        {
            var success = await subcategoryRepository   .DeleteAsync(subcategoryId);

            if (success)
                return Ok($"Subcategory with ID {subcategoryId} deleted successfully.");
            else
                return NotFound($"Subcategory with ID {subcategoryId} not found.");
        }
        [HttpPut("toggle-archive/{categoryId}")]
        public async Task<IActionResult> ToggleArchive(int categoryId)
        {
            try
            {
                var success = await subcategoryRepository.ToggleArchivedAsync(categoryId);

                if (success)
                {
                    return Ok($"L'archivage de la catégorie avec l'ID {categoryId} a été modifié avec succès.");
                }
                else
                {
                    return NotFound($"Catégorie avec l'ID {categoryId} non trouvée.");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception according to your needs
                return StatusCode(500, $"Erreur lors de la modification de l'archivage de la catégorie. Détails : {ex.Message}");
            }
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
                var result = await subcategoryRepository.DeleteMultipleAsync(subcategoryIds);

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
                var result = await subcategoryRepository.DeleteAllAsync();

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
