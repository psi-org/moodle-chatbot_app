using System.Data;

namespace MoodleBot.Entity
{
    public class EntityFrameworkStorageOptions
    {
        /// <summary>
        /// Gets or sets the connection string to use while creating BotDataContext.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the transaction isolation level to use during write operations.
        /// </summary>
        /// <remarks>
        /// Default IsolationLevel.ReadCommitted
        /// </remarks>
        public IsolationLevel TransactionIsolationLevel => IsolationLevel.ReadCommitted;
    }
}
