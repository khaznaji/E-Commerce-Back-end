using E_Commerce.Models.Domain;

namespace E_Commerce.Repositories.Interface
{
    public interface IFournisseurRepository
    {
        Task<Fournisseur> CreateAsync(Fournisseur fournisseur);


    }
}
