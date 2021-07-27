using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SAModels {
    public class Category : IStoreModel{
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
