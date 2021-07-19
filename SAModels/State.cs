using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public class State : StoreModel {
        [Key]
        [Required]
        [MaxLength(2)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public override int GetID() {
            int sum = 0;
            foreach (int i in Code.ToCharArray()) {
                sum += i;
            }
            return sum;
        }
    }
}
