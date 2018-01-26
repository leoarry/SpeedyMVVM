using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    public interface IRepositoryService<T> where T:EntityBase
    {
        /// <summary>
        /// Queryable collection of entities.
        /// </summary>
        IQueryable<T> DataSet { get; set; }

        /// <summary>
        /// Add a new entity to the data center.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <returns>Entity added.</returns>
        T Add(T entity);

        /// <summary>
        /// Remove an existing entity from the data center.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        /// <returns>Entity removed.</returns>
        T Remove(T entity);

        /// <summary>
        /// Persist the data into the data center.
        /// </summary>
        /// <returns>Return '0' in case of failure.</returns>
        int Save(T entity = null);

        /// <summary>
        /// Retrieve a collection from the data center of specified type 'TEntity' based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        Task<ObservableCollection<TEntity>> RetrieveCollection<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;

        /// <summary>
        /// Retrieve a collection from the data center based on the 'Expression' parameter.
        /// </summary>
        /// <param name="predicate">Expression to use.</param>
        /// <returns>Result of the query</returns>
        Task<ObservableCollection<T>> RetrieveCollection(Expression<Func<T, bool>> predicate);
    }
}
