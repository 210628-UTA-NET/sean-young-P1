using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class Product {

        private decimal _Price;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        [Column(TypeName = "money")]
        public decimal Price { 
            get { return _Price; } 
            set {
                if (value < 0) throw new ArgumentException("A product cannot have a negative price.");

                _Price = value;
            } 
        }

        public string Description { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
