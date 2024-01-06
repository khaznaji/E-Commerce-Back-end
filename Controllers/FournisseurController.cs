using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Implementation;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FournisseurController : ControllerBase
    {
        private readonly IFournisseurRepository fournisseurRepository;

        public FournisseurController(IFournisseurRepository fournisseurRepository)
        {
            this.fournisseurRepository = fournisseurRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFournisseur([FromForm] Fournisseur fournisseur)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    // Call the repository's CreateAsync method with the image file
                    await fournisseurRepository.CreateAsync(fournisseur);

                    return Ok();
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.Error.WriteLine(ex);
                return StatusCode(500, "An error occurred during fournisseur creation.");
            }
        }




    }
}
