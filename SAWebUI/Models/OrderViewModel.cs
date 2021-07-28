using System.Collections.Generic;
using SAModels;

namespace SAWebUI.Models {
    public class OrderViewModel {
        public IList<Order> Orders { get; set; }
        public string OrderBy { get; set; }
        public string Title { get; set; }

    }
}
