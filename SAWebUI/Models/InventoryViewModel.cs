using SAModels;
using System.Collections.Generic;
namespace SAWebUI.Models {
    public class InventoryViewModel {
        public IList<LineItem> Inventory { get; set; }
        public string StatusMessage { get; set; }
    }
}