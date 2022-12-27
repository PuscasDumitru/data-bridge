using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected RepositoryDbContext RepositoryContext { get; set; }

        public GenericRepository(RepositoryDbContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> GetAll()
        {
            return RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return RepositoryContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public T UpdateEntity(T originalEntity, T updateEntity)
        {
            foreach (var property in updateEntity.GetType().GetProperties())
            {
                if (property.GetValue(updateEntity, null) == null)
                {
                    property.SetValue(updateEntity, originalEntity.GetType().GetProperty(property.Name)
                        .GetValue(originalEntity, null));
                }
            }
            return updateEntity;
        }

    }
}
