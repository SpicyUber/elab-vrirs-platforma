using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly VrirsDbContext context;

        public GenericRepository(VrirsDbContext context)
        {
            this.context = context;
        }
        public void Add(T entity)
        {
           context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public T GetById(Guid id)
        {
            return context.Set<T>().Find(id);
        }

        public IQueryable<T> Query()
        {
            return context.Set<T>();
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
    }
}
