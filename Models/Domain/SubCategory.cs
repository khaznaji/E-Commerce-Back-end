namespace E_Commerce.Models.Domain
{
    public class SubCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; } // Foreign key referencing the parent category
        public Category? Category { get; set; } // Navigation property to the parent category
        public DateTime Date { get; set; } = DateTime.Now; // Initialize with the current date
        public bool Archive { get; set; }
    }
}
