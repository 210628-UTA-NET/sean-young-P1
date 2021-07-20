using System;
using System.ComponentModel.DataAnnotations;

namespace SAWebUI.Models {
    public class HomeViewModel {

        [Required]
        public string SearchString { get; set; }

    }
}
