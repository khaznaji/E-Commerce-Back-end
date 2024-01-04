using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;


        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User model)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Un utilisateur avec cet e-mail existe déjà.");
            }

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var newUser = new User
            {
                Email = model.Email,
                Password = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                AddressLine1 = model.AddressLine1,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };

            await _userRepository.CreateUserAsync(newUser);

            return Ok("Inscription réussie.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            var isLoginSuccessful = await _userRepository.LoginAsync(model.Email, model.Password);

            if (isLoginSuccessful)
            {
                // TODO: Generate and return a token for authentication
                return Ok("Login successful");
            }
            else
            {
                return BadRequest("Invalid email or password");
            }
        }
    }
}
