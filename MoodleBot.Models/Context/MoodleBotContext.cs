using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MoodleBot.Common;
using static MoodleBot.Models.BotDataEntity;
using static MoodleBot.Models.BotEmojis;
using static MoodleBot.Models.BotMessages;

namespace MoodleBot.Models.Context
{
    public class MoodleBotContext : DbContext, IDbContext
    {
        #region Properties
        //private IDbContextTransaction _transaction;
        private ILogger _logger;
        private IConcurrencyExceptionHandler _concurrencyExceptionHandler;
        #endregion

        #region Constructor
        public MoodleBotContext(DbContextOptions options, ILogger logger, IConcurrencyExceptionHandler concurrencyExceptionHandler) : base(options)
        {
            _logger = logger;
            _concurrencyExceptionHandler = concurrencyExceptionHandler;
        }
        #endregion

        #region Set DBContext Model
        public DbSet<BotMessages> BotMessages { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<BotDataEntity> BotDataEntity { get; set; }
        public DbSet<BotEmojis> BotEmojis { get; set; }
        public DbSet<UserCreationQuestion> UserCreationQuestion { get; set; }
        public DbSet<CountryLanguage> CountryLanguage { get; set; }
        #endregion

        public override int SaveChanges()
        {
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                var additionalInfo = _concurrencyExceptionHandler.GetAdditionalInfo(exception);
                _logger.Error($"Concurrency exception in SaveChanges() {additionalInfo}", exception);
                throw; // caller should handle it, we are just logging extra info here
            }
            catch (Exception exception)
            {
                _logger.Error("Error Saving TO DB", exception);
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                return await base.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                var additionalInfo = _concurrencyExceptionHandler.GetAdditionalInfo(exception);
                _logger.Error($"Concurrency exception in SaveChanges() {additionalInfo}", exception);
                throw; // caller should handle it, we are just logging extra info here
            }
            catch (Exception exception)
            {
                _logger.Error("Error Saving TO DB", exception);
                throw;
            }
        }

        public void BeginTransaction()
        {
            //_transaction = base.Database.BeginTransaction();
        }

        public void Commit()
        {
            //_transaction.Commit();
        }

        public void Rollback()
        {
            //_transaction.Rollback();
        }

        public void Detach<TEntity>(TEntity entity)
        {
            base.Entry(entity).State = EntityState.Detached;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BotDataEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BotEmojisConfiguration());
        }
    }
}
