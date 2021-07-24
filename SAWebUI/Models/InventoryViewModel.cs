using SAModels;
using System.Collections.Generic;
namespace SAWebUI.Models {
    public class InventoryIndexViewModel {
        public string PreviousURL { get; set; }
        public IList<LineItem> Inventory { get; set; }
    }
}