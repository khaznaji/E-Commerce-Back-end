using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;



        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.User.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return false; // User not found

            // Verify the entered password
            bool isPasswordValid = VerifyPassword(password, user.Password);

            return isPasswordValid;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Validate stored password format
            if (!storedPassword.Contains("."))
            {
                // Handle the case where the stored password is not in the expected format
                return false;
            }

            // Extract the salt from the stored password
            byte[] salt = Convert.FromBase64String(storedPassword.Split('.')[0]);

            // Hash the entered password with the extracted salt
            string hashedEnteredPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hashed entered password with the stored password
            return hashedEnteredPassword == storedPassword;
        }





    }
}
