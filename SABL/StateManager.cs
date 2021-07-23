using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class StateManager {
        private readonly ICRUD<State> _db;
        public StateManager(ICRUD<State> p_db) {
            _db = p_db;
        }

        public IList<State> GetAllStates() {
            return _db.Query(new(null){});
        }
    }
}