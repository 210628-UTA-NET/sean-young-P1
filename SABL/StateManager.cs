using SADL;
using SAModels;
using System.Collections.Generic;

namespace SABL {
    /// <summary>
    /// Business Layer class which manages the querying of the States that
    /// are used in the Address Model.
    /// </summary>
    public class StateManager {
        private readonly ICRUD<State> _db;

        /// <param name="p_db">Data Layer interface that can perform CRUD operations on States</param>
        public StateManager(ICRUD<State> p_db) {
            _db = p_db;
        }

        /// <summary>
        /// Queries all states from the database
        /// </summary>
        /// <returns>A List containing all of the states in the database</returns>
        public IList<State> GetAllStates() {
            return _db.Query(new(null){});
        }
    }
}