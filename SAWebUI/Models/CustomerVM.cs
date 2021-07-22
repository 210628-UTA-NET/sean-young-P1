using System;
using System.Text;
using SAModels;

namespace SAWebUI.Models {
    public class CustomerVM {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public CustomerVM(CustomerUser p_user) {
            StringBuilder sb = new();
            sb.Append(p_user.FirstName ?? "");
            sb.AppendFormat(" {0}", p_user.LastName ?? "");
            Id = p_user.Id;
            Name = sb.ToString();
            Address = (p_user.Address == null) ? "None" : p_user.Address.ToString();
            Email = (p_user.Email == null) ? "None" : p_user.Email;
            Phone = (p_user.PhoneNumber == null) ? "None" : p_user.PhoneNumber;
        }
    }
}
