using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using LDevelopment.Interfaces;
using LDevelopment.Context;

namespace LDevelopment.Repositories
{
    public class Repository : IRepository, IDisposable
    {
        public Repository()
        {
            this.Context = new ApplicationDbContext();
        }

        public ApplicationDbContext Context { get; private set; }

        public T Find<T>(int id, params Expression<Func<T, object>>[] items) where T : class, IModel
        {
            return Find<T>(x => x.Id == id, items);
        }

        public T Find<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] items) where T : class, IModel
        {
            var result = Context.Set<T>() as IQueryable<T>;

            result = items.Aggregate(result, (current, item) => current.Include(item));

            return result.FirstOrDefault(predicate);
        }

        public IQueryable<T> All<T>(Expression<Func<T, bool>> predicate = null) where T : class, IModel
        {
            IQueryable<T> result = Context.Set<T>().Where(x => x.IsDeleted != true);

            return predicate != null ? result.Where(predicate) : result;
        }

        public void Add<T>(T model) where T : class, IModel
        {
            Context.Set<T>().Add(model);
        }

        public void Update<T>(T model) where T : class, IModel
        {
            Context.Entry(model).State = EntityState.Modified;
        }

        public void Delete<T>(int id) where T : class, IModel
        {
            T model = Find<T>(id);

            model.IsDeleted = true;
            model.DeletedDate = DateTime.UtcNow;

            Update(model);
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}