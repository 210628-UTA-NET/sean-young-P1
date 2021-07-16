using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAModels {
    public class Customer : StoreModel {

        private string _Name;
        private MailAddress _EmailAddress;
        private string _Phone;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name {
            get { return _Name; }
            set {
                if (Regex.IsMatch(value, @"^[a-zA-Z\s]+$")) {
                    _Name = value;
                } else {
                    throw new FormatException("A customer name can contain only characters and spaces.");
                }
            }
        }

        [Required]
        public Address Address { get; set; }

        [Required]
        [MaxLength(320)]
        public string Email {
            get {
                return _EmailAddress?.ToString();
            }
            set {
                _EmailAddress = new MailAddress(value);
            }
        }

        [Required]
        [MaxLength(25)]
        public string Phone {
            get { return _Phone; }
            set {
                if (Regex.IsMatch(value, @"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")) {
                    _Phone = value;
                } else {
                    throw new FormatException("Invalid phone number format");
                }
            }
        }
    }
}
