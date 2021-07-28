using System.Collections.Generic;


namespace SADL {
    /// <summary>
    /// Interface which defines the basic CRUD operations that can be performed
    /// on the various store app models. 
    /// </summary>
    /// <typeparam name="T">Any derived class of IStoreModel</typeparam>
    public interface ICRUD<T> where T : class, SAModels.IStoreModel {

        /// <summary>
        /// Inserts the model into the database.
        /// </summary>
        /// <param name="p_model">The model to insert</param>
        void Create(T p_model);

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
        IList<T> Query(QueryOptions<T> p_options);

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
        T FindSingle(QueryOptions<T> p_options);

        /// <summary>
        /// Will update the model with the given ID with any non-null values.
        /// Any null values will not be set as null but rather be ignored such
        /// that any existing values will remain unchanged. Non-null values
        /// will be changed.
        /// </summary>
        /// <param name="p_model">The model with Id to be updated</param>
        void Update(T p_model);

        /// <summary>
        /// Deletes the given model from the database
        /// </summary>
        /// <param name="p_model">The model to delete from the database</param>
        void Delete(T p_model);

        /// <summary>
        /// Saves all changes to tracked objects to the database.
        /// </summary>
        void Save();

        /// <summary>
        /// Flags an object for deletion upon the next call to Save().
        /// </summary>
        /// <param name="p_model">The object to mark for deletion</param>
        void FlagForRemoval(T p_model);
    }
}
