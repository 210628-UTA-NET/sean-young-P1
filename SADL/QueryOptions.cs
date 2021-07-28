using Microsoft.Extensions.Configuration;
using SAModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SADL {
    public enum OrderBy {
        None,
        Ascending,
        Descending,
    }

    /// <summary>
    /// A class which bundles together the various parameters that the user can
    /// specify to query the database. These parameters include which related
    /// entities to load, conditions on properties, pagination, and sorted order.
    /// </summary>
    /// <typeparam name="T">The type of StoreModel</typeparam>
    public class QueryOptions<T> where T : class, SAModels.IStoreModel {
        private readonly SAOptions _pageSizeOptions;
        public IList<Func<T, bool>> Conditions { get; set; }
        public IList<string> Includes { get; set; }

        [Range(0, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(0, int.MaxValue)]
        public int PageSize { get; private set; }
        public bool Paged { get; set; } = false;
        public (OrderBy, string) SortOrder { get; set; }

        /// <summary>
        /// Initializes the queryoptions and loads configuration from the 
        /// appsettings.json.
        /// </summary>
        /// <param name="p_configuration">
        /// Configuration from the appsettings.json
        /// </param>
        public QueryOptions(IConfiguration p_configuration) {
            if (p_configuration != null) {
                _pageSizeOptions = new SAOptions();
                p_configuration.GetSection(SAOptions.PageOptions).Bind(_pageSizeOptions);
                PageSize = _pageSizeOptions.PageSize;
            } else {
                Paged = false;
            }
            
            Conditions = new List<Func<T, bool>>();
            Includes = new List<string>();
            SortOrder = (OrderBy.None, null);
        }

        /// <summary>
        /// Parses the given input string to determine the column and order by
        /// which the query results should be sorted.
        /// </summary>
        /// <param name="p_inputString">
        /// A string specifying the column and order
        /// </param>
        public void AddParseSortOrder(string p_inputString) {
            string[] tokens = p_inputString.Split('_'); // LastName_desc
            if (tokens.Length != 2) throw new ArgumentException("Malformed sorting order string.");

            OrderBy o = tokens[1] switch {
                "desc" => OrderBy.Descending,
                "asc" => OrderBy.Ascending,
                _ => throw new ArgumentException("Order string must be <COLUMNNAME>_asc, or <COLUMNNAME_desc."),
            };
            SortOrder = (o, tokens[0]);
        }
    }
}