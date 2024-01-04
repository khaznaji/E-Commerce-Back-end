namespace E_Commerce.Models.Domain
{
        public class User
        {
        public int Id { get; set; }

        // Informations de connexion
        public string Email { get; set; }
        public string Password { get; set; }

        // Informations personnelles
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }

        // Adresse
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
