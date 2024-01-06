using E_Commerce.Data;
using E_Commerce.Models.Domain;
using E_Commerce.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;



namespace E_Commerce.Repositories.Implementation
{
    public class FournisseurRespository : IFournisseurRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FournisseurRespository(ApplicationDbContext context)
        { this._context = context; }

        public async Task<Fournisseur> CreateAsync(Fournisseur fournisseur)
        {
            var existingFournisseur = await _context.Categories.FirstOrDefaultAsync(c => c.Name == fournisseur.Name);

            if (existingFournisseur != null)
            {
                // Category with the same name already exists, return a conflict response
                // You can customize the response based on your application's requirements
                return null;
            }
            _context.Fournisseurs.AddAsync(fournisseur);
            await _context.SaveChangesAsync();
            return fournisseur;

        }
    }
}
