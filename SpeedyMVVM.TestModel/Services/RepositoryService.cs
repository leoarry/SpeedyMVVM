using SpeedyMVVM.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace SpeedyMVVM.TestModel.Services
{
    public class RepositoryService<T> : IRepositoryService<T> where T : IEntityBase
    {
        public List<T> List { get; set; }

        public RepositoryService()
        {
            List = new List<T>();
        }

        public T AddEntity(T entity)
        {
            entity.ID = List.Max(u => u.ID);
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

        public ObservableCollection<TEntity> RetrieveCollection<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : IEntityBase
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<T>> RetrieveCollectionAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.Factory.StartNew(() => RetrieveCollection(predicate));
        }

        public Task<ObservableCollection<TEntity>> RetrieveCollectionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : IEntityBase
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
            var entityToUpdate = List.Find(e=> e.ID==entity.ID);
            entityToUpdate = entity;
            return entityToUpdate;
        }

        public Task<T> UpdateEntityAsync(T entity)
        {
            return Task.Factory.StartNew(() => UpdateEntity(entity));
        }
    }
}
