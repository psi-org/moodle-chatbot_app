using Microsoft.EntityFrameworkCore;
using MoodleBot.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBContext
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {

        virtual protected DbSet<T> DbSet { get; set; }

        public RepositoryBase()
        {
            //DbContext = dbContext;
            //DbSet = dbContext.Set<T>();
        }

        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            T entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(T entityToDelete)
        {
            DbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetManyQueryable(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where);
        }

        public virtual IQueryable<T> FromSql(FormattableString sql)
        {
            var query = DbSet.FromSqlInterpolated<T>(sql);
            return query;
        }

        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Get(Func<T, bool> where)
        {
            return DbSet.Where(where).FirstOrDefault<T>();
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(Func<T, bool> where)
        {
            IQueryable<T> objects = DbSet.Where<T>(where).AsQueryable();
            foreach (T obj in objects)
                DbSet.Remove(obj);
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<T> GetWithInclude(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params string[] include)
        {
            IQueryable<T> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        public IQueryable<T> GetWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProps)
        {
            IQueryable<T> query = DbSet.Where(predicate);
            query = includeProps.Aggregate(query, (current, inc) => current.Include(inc));

            return query;

        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T GetSingle(Func<T, bool> predicate)
        {
            return DbSet.Single<T>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirst(Func<T, bool> predicate)
        {
            return DbSet.First<T>(predicate);
        }

        /// <summary>
        /// Saves all pending db changes and ends the transaction
        /// </summary>
        public void SaveChanges()
        {
            //DbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            //await DbContext.SaveChangesAsync();
        }

        public Task<List<T>> ToListAsync(IQueryable<T> queryable)
        {
            return queryable.ToListAsync();
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void Detach(T entity)
        {
            //DbContext.Detach<T>(entity);
        }
    }
}
