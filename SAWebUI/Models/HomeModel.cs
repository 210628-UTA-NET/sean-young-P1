using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SAModels;

namespace SAWebUI.Models {
    public class HomeModel {

        [Required]
        public string SearchString { get; set; }

        public IEnumerable<Storefront> Storefronts { get; set;  }

        public bool hasStorefronts() {
            return Storefronts != null; 
        }

    }
}