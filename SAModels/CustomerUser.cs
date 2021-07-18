using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SAModels {
    public class CustomerUser : IdentityUser {
        private string _FirstName;
        private string _LastName;

        [MaxLength(100)]
        public string FirstName {
            get { return _FirstName; }
            set {
                if (Regex.IsMatch(value, @"^[a-zA-Z\s]+$")) {
                    _FirstName = value;
                } else {
                    throw new FormatException("A customer name can contain only characters and spaces.");
                }
            }
        }

        [MaxLength(100)]
        public string LastName {
            get { return _LastName; }
            set {
                if (Regex.IsMatch(value, @"^[a-zA-Z\s]+$")) {
                    _LastName = value;
                } else {
                    throw new FormatException("A customer name can contain only characters and spaces.");
                }
            }
        }

        public Address Address { get; set; }
    }
}
