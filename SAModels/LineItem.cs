using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class LineItem : IStoreModel{
        private int _Quantity;

        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Product Product { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity {
            get { return _Quantity; }
            set {
                    if (value < 0) throw new ArgumentException("A line item cannot have a negative quantity.");
                    _Quantity = value;
            }
        }

        public int? OrderId { get; set; }
        public int? ShoppingCardId { get; set; }
        public int? StorefrontId { get; set; }
    }
}
