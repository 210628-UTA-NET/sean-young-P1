using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class Storefront : StoreModel{
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
