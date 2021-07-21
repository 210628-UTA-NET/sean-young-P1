using System;
using System.Collections.Generic;

namespace SADL {
    public enum OrderBy {
        Ascending,
        Descending,
    }
    public class QueryOptions<T> where T : class, SAModels.IStoreModel {
        public IList<Func<T, bool>> Conditions { get; set; }
        public IList<string> Includes { get; set; }
        public int Page { get; set; }
        public int PageSize { get; private set; }
        public bool Single { get; set; }
        public (OrderBy, string) Order { get; set; }
    }
}