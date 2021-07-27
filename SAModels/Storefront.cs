using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SAModels {
    public class Storefront : IStoreModel{
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public virtual Address Address { get; set; }

        public virtual ICollection<LineItem> Items { get; set; }
    }
}
