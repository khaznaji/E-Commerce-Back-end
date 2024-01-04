using E_Commerce.Models.Domain;


namespace E_Commerce.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task CreateUserAsync(User user);
        Task<bool> LoginAsync(string email, string password);

    }
}
