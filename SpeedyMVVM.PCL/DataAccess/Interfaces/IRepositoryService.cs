using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess.Interfaces
{
    public interface IRepositoryService<T> where T : EntityBase
    {

        #region Properties
        IQueryable<T> DataSet { get; set; }
        #endregion

        #region Create Methods
        /// <summary>
        /// Add a new entity to the data center.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <returns>Entity added.</returns>
        T AddEntity(T entity);

        /// <summary>
        /// ASYNC - Add a new entity to the data center.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <returns>Entity added.</returns>
        Task<T> AddEntityAsync(T entity);
        #endregion

        #region Read Methods
        /// <summary>
        /// Retrieve a collection from the data center based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        ObservableCollection<T> RetrieveCollection(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// ASYNC - Retrieve a collection from the data center based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        Task<ObservableCollection<T>> RetrieveCollectionAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieve a collection from the data center of specified type 'TEntity' based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        ObservableCollection<TEntity> RetrieveCollection<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;

        /// <summary>
        /// ASYNC - Retrieve a collection from the data center of specified type 'TEntity' based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        Task<ObservableCollection<TEntity>> RetrieveCollectionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;
        #endregion

        #region Update Methods
        /// <summary>
        /// Update an existing entity.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <returns>Updated Entity.</returns>
        T UpdateEntity(T entity);

        /// <summary>
        /// ASYNC - Update an existing entity.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <returns>Updated Entity.</returns>
        Task<T> UpdateEntityAsync(T entity);
        #endregion

        #region Delete Methods
        /// <summary>
        /// Remove an existing entity from the data center.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        /// <returns>Entity removed.</returns>
        T RemoveEntity(T entity);

        /// <summary>
        /// ASYNC - Remove an existing entity from the data center.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        /// <returns>Entity removed.</returns>
        Task<T> RemoveEntityAsync(T entity);
        #endregion

        #region Persist Data Methods
        /// <summary>
        /// Persist the data into the data center.
        /// </summary>
        /// <returns>Return '0' in case of failure.</returns>
        int SaveChanges();

        /// <summary>
        /// ASYNC - Persist the data into the data center.
        /// </summary>
        /// <returns>Return '0' in case of failure.</returns>
        Task<int> SaveChangesAsync();
        #endregion
    }
}
