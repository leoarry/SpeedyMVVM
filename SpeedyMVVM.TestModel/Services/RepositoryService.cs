using SpeedyMVVM.DataAccess.Interfaces;
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

        public T AddEntity(T entity)
        {
            List.Add(entity);
            return entity;
        }

        public Task<T> AddEntityAsync(T entity)
        {
            return Task.Factory.StartNew(()=> AddEntity(entity));
        }

        public T RemoveEntity(T entity)
        {
            List.Remove(entity);
            return entity;
        }

        public Task<T> RemoveEntityAsync(T entity)
        {
            return Task.Factory.StartNew(() => RemoveEntity(entity));
        }

        public ObservableCollection<T> RetrieveCollection(Expression<Func<T, bool>> predicate)
        {
            var result = List.AsQueryable().Where(predicate);
            return (result != null) ? new ObservableCollection<T>(result) : new ObservableCollection<T>();
        }

        public ObservableCollection<TEntity> RetrieveCollection<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<T>> RetrieveCollectionAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.Factory.StartNew(() => RetrieveCollection(predicate));
        }

        public Task<ObservableCollection<TEntity>> RetrieveCollectionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
        {
            return Task.Factory.StartNew(() => RetrieveCollection<TEntity>(predicate));
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.Factory.StartNew(() => SaveChanges());
        }

        public T UpdateEntity(T entity)
        {
            var entityToUpdate = List.Find(e=> e==entity);
            entityToUpdate = entity;
            return entityToUpdate;
        }

        public Task<T> UpdateEntityAsync(T entity)
        {
            return Task.Factory.StartNew(() => UpdateEntity(entity));
        }
    }
}
