using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SAModels {
    public abstract class StoreModel {
        public void Transfer(StoreModel p_other) {
            if (p_other.GetType() == GetType()) {
                foreach(PropertyInfo prop in GetType().GetProperties()) {
                    prop.SetValue(p_other, prop.GetValue(this));
                }
            } else {
                throw new ArgumentException("StoreModel must be the same type.");
            }
        }
    }
}
