using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SADL {
    /// <summary>
    /// Implementation of the ICRUD interface which performs the various CRUD
    /// operations using EF Core.
    /// </summary>
    /// <typeparam name="T">Any derived class of IStoreModel</typeparam>
    public class StoreModelDB<T>: ICRUD<T> where T : class, SAModels.IStoreModel{

        private readonly SADBContext _context;

        public StoreModelDB(SADBContext p_context) {
            _context = p_context;
        }

        /// <summary>
        /// Inserts the model into the database.
        /// </summary>
        /// <param name="p_model">The model to insert</param>
        public void Create(T p_model) {
            _context.Set<T>().Add(p_model);
            _context.SaveChanges();
        }


        /// <summary>
        /// Deletes the given model from the database
        /// </summary>
        /// <param name="p_model">The model to delete from the database</param>
        public void Delete(T p_model) {
            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Remove(p_model);
            _context.SaveChanges();

        }

        /// <summary>
        /// Query the database for models which meet the the user specified 
        /// requirements defined in the QueryOptions class. Requirements 
        /// include conditions, includes (eager loading), and pagination.
        /// </summary>
        /// <param name="p_options">
        /// A class that wraps the various parameters that the user can apply 
        /// to a query
        /// </param>
        /// <returns>A list of T storemodels which meet the requirements</returns>
        public IList<T> Query(QueryOptions<T> p_options) {
            if (p_options.Page < 0 || p_options.PageSize < 0) throw new ArgumentException("Cannot have a negative page size or navigate to a negative page.");

            // Load relations and subrelations
            var queryableQuery = _context.Set<T>().AsQueryable();
            if (p_options.Includes != null) {
                foreach (string inc in p_options.Includes) {
                    queryableQuery = queryableQuery.Include(inc);
                }
            }

            // Add conditions
            var enumerableQuery = queryableQuery.AsEnumerable();
            if (p_options.Conditions != null) {
                foreach (Func<T, bool> cond in p_options.Conditions) {
                    enumerableQuery = enumerableQuery.Where(cond);
                }
            }

            // Paging if applicable
            if (p_options.Paged) {
                int skip = (p_options.Page - 1) * p_options.PageSize;
                enumerableQuery = enumerableQuery.Select(o => o).Skip(skip).Take(p_options.PageSize);
            }

            // Ordering if applicable
            (OrderBy option, string columnName) = p_options.SortOrder;
            if (option == OrderBy.Ascending) {
                enumerableQuery = enumerableQuery.OrderBy(i => typeof(T).GetProperty(columnName).GetValue(i).ToString());
            } else if (option == OrderBy.Descending) {
                enumerableQuery = enumerableQuery.OrderByDescending(i => typeof(T).GetProperty(columnName).GetValue(i).ToString());
            }

            return enumerableQuery.Select(o => o).ToList();
        }

        /// <summary>
        /// Returns a single instance from the database which meets the 
        /// requirements. Will return the first match if multiple rows meet the
        /// requirements.
        /// </summary>
        /// <param name="p_options">
        /// A class that wraps the various parameters that the user can apply 
        /// to a query
        /// </param>
        /// <returns>A single T storemeodel which meets the requirements</returns>
        public T FindSingle(QueryOptions<T> p_options) {
            return Query(p_options).FirstOrDefault();
        }

        /// <summary>
        /// Will update the model with the given ID with any non-null values.
        /// Any null values will not be set as null but rather be ignored such
        /// that any existing values will remain unchanged. Non-null values
        /// will be changed.
        /// </summary>
        /// <param name="p_model">The model with Id to be updated</param>
        public void Update(T p_model) {
            _context.Set<T>().Attach(p_model);

            var entry = _context.Entry(p_model);
            entry.State = EntityState.Modified;

            foreach(var prop in entry.Properties.Where(p => !p.Metadata.IsKey()) ) {
                if (prop.CurrentValue == null) {
                    prop.IsModified = false;
                } 
            }

            _context.SaveChanges();
            entry.State = EntityState.Detached;
        }

        /// <summary>
        /// Saves all changes to tracked objects to the database.
        /// </summary>
        public void Save() {
            _context.SaveChanges();
        }

        /// <summary>
        /// Flags an object for deletion upon the next call to Save().
        /// </summary>
        /// <param name="p_model">The object to mark for deletion</param>
        public void FlagForRemoval(T p_model) {
            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Remove(p_model);
        }
    }
}
