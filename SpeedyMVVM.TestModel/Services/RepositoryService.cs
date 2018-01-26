using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using SpeedyMVVM.DataAccess;

namespace SpeedyMVVM.TestModel.Services
{
    public class RepositoryService<T> : IRepositoryService<T> where T : EntityBase
    {
        public List<T> List { get; set; }

        public IQueryable<T> DataSet
        {
            get
            {
                return List.AsQueryable();
            }

            set
            {
                List=value.ToList();
            }
        }

        public RepositoryService()
        {
            List = new List<T>();
        }

        public T Add(T entity)
        {
            List.Add(entity);
            return entity;
        }
        
        public T Remove(T entity)
        {
            List.Remove(entity);
            return entity;
        }
        
        public int Save(T entity)
        {
            return 0;
        }

        Task<ObservableCollection<TEntity>> IRepositoryService<T>.RetrieveCollection<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return null;
        }

        Task<ObservableCollection<T>> IRepositoryService<T>.RetrieveCollection(Expression<Func<T, bool>> predicate)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = List.AsQueryable().Where(predicate);
                return (result != null) ? new ObservableCollection<T>(result) : new ObservableCollection<T>();
            });
        }
    }
}
