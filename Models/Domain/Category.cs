namespace E_Commerce.Models.Domain
{
    public class Category
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Date { get; set; } = DateTime.Now; // Initialise avec la date actuelle
        public bool Archive { get; set; }
        public ICollection<SubCategory>? Subcategories { get; set; }


    }
}
