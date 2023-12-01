using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Implementation;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRespository categoryRespository;
        public CategoriesController(ICategoryRespository categoryRespository)
        {
            this.categoryRespository = categoryRespository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await categoryRespository.CreateAsync(category);

                return Ok();
            }

            return BadRequest(ModelState);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = await categoryRespository.GetAllAsync();
            return Ok(categories);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await categoryRespository.DeleteAsync(id);

            if (isDeleted)
                return Ok();

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return BadRequest("Mismatched IDs in the request.");
                }

                var updatedCategory = await categoryRespository.UpdateAsync(category);

                if (updatedCategory == null)
                {
                    return NotFound("Category not found.");
                }

                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.Error.WriteLine(ex);
                return StatusCode(500, "An error occurred during the update.");
            }
        }
    }
}
