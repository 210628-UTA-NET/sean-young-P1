using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAModels {
    public class Address {

        private string _ZipCode;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string StreetAddress { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        public State State { get; set; }

        [Required]
        public Country Country { get; set; }

        [MaxLength(10)]
        [RegularExpression("^[0-9]*$")]
        public string ZipCode {
            get { return _ZipCode; }
            set {
                if ((value.Length != 5) || Regex.IsMatch(value, "^[0-9]*$"))
                    throw new FormatException("A zip code must be 5 digits. Example: '94506'");

                _ZipCode = value;
            }
        }

    }
}
