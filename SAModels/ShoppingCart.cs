using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SAModels {
    public class ShoppingCart : IStoreModel{
        private decimal _TotalAmount;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal TotalAmount {
            get { return _TotalAmount; }
            set {
                if (value < 0) throw new ArgumentException("Order cannot have a negative total.");
                _TotalAmount = value;
            }
        }

        [Required]
        public virtual CustomerUser CustomerUser { get; set; }
        public virtual Storefront Storefront { get; set; }
        public virtual ICollection<LineItem> Items { get; set; }

        public string CustomerUserId { get; set; }
        public int StorefrontId { get; set; }
    }
}
