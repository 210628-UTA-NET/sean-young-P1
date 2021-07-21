using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAModels {
    public class Address : StoreModel {
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
        public virtual State State { get; set; }

        [MaxLength(10)]
        [RegularExpression(@"^\\d+$")]
        public string ZipCode {
            get { return _ZipCode; }
            set {
                if ((value.Length != 5) || Regex.IsMatch(value, @"^\\d+$"))
                    throw new FormatException("A zip code must be 5 digits. Example: '94506'");

                _ZipCode = value;
            }
        }

        public override string ToString() {
            StringBuilder sb = new();
            // Hardcoded --> Move to config
            if (StreetAddress.Length > 50) {
                sb.AppendFormat(StreetAddress.Substring(0, 50));
                sb.Append("...");
            }
            sb.Append(StreetAddress);

            sb.AppendFormat(", {0}, {1}, {2}", City, State.Code, ZipCode);

            return sb.ToString();
        }

        public string AddressSecond() {
            return string.Format("{0}, {1}, {2}", City, State.Code, ZipCode);
        }
    }
}
