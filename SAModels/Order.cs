using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class Order {

        private decimal _TotalAmount;

        [Key]
        public int Id { get; set; }

        [Required]
        public Customer Customer { get; set; }

        [Required]
        public Storefront Storefront { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal TotalAmount {
            get { return _TotalAmount; } 
            set {
                if (value < 0) throw new ArgumentException("Order cannot have a negative total.");
                _TotalAmount = value;
            } 
        }

        public virtual ICollection<LineItem> LineItems { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DatePlaced { get; set; }
    }
}
