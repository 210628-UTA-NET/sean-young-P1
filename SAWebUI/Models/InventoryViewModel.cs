using SAModels;
using System.Collections.Generic;
namespace SAWebUI.Models {
    public class InventoryIndexViewModel {
        public string SearchString { get; set; }

        public IList<Category> Categories { get; set; }
    }
}