using MoodleBot.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBContext
{
    public interface IRepositoryBase<T>
    {
        //IDbContext DbContext { get; }
        void Insert(T entity);
        bool Exists(object primaryKey);
        T Get(Func<T, bool> where);
        IQueryable<T> GetAll();
        T GetById(object id);
        T GetFirst(Func<T, bool> predicate);
        IQueryable<T> GetManyQueryable(Expression<Func<T, bool>> where);
        IQueryable<T> FromSql(FormattableString sql);
        T GetSingle(Func<T, bool> predicate);
        IQueryable<T> GetWithInclude(Expression<Func<T, bool>> predicate, params string[] include);
        IQueryable<T> GetWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProps);
        void Delete(T entityToDelete);
        void Delete(object id);
        void Delete(Func<T, bool> where);
        void SaveChanges();
        Task SaveChangesAsync();
        void Update(T entity);
        void Detach(T entity);
        Task<List<T>> ToListAsync(IQueryable<T> queryable);
    }
}
