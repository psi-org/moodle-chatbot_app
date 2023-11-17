using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoodleBot.Models.Context
{
    public interface IDbContext : ITransientDbContext
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        void BeginTransaction();

        void Commit();

        void Rollback();

        void Detach<TEntity>(TEntity entity);
    }

    public interface ITransientDbContext : IDisposable
    {

    }
}
