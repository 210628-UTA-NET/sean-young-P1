using System;
using System.ComponentModel.DataAnnotations;


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
                    if (value < 0) throw new ArgumentException("A line item cannot have a negative quantity");
                    _Quantity = value;
            }
        }

        public int? OrderId { get; set; }
        public int? ShoppingCartId { get; set; }
        public int? StorefrontId { get; set; }
    }
}
