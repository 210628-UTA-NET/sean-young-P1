using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class Country {
        [Key]
        [Required]
        [MaxLength(2)]
        public string Alpha2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
