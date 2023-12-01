﻿using E_Commerce.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products {  get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
