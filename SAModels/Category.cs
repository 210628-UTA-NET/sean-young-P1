﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class Category : StoreModel{
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public override int GetID() {
            return Id;
        }
    }
}
