using System;

namespace SAWebUI.Models {
    public class StorefrontViewModel {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AddressId { get; set; }
        public string AddressStreetAddress { get; set; }
        public string AddressCity { get; set; }
        public string AddressStateCode { get; set; }
        public string AddressStateName { get; set; }
        public string AddressCountryName { get; set; }
        public string AddressZipCode { get; set; }

    }
}
