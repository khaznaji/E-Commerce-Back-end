using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.Domain
{
    public class Fournisseur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cin { get; set; }  
        public int Tel { get; set; }
        public string sexe { get; set; }
    }
}
