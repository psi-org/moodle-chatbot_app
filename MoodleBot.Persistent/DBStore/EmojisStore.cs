using Microsoft.EntityFrameworkCore;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public class EmojisStore : IEmojisStore
    {
        #region Properties
        private readonly IRepositoryBase<BotEmojis> _repoEmojis;
        #endregion

        #region Constructor
        public EmojisStore(IRepositoryBase<BotEmojis> repoEmojis)
        {
            _repoEmojis = repoEmojis;
        }
        #endregion

        #region Public Method
        public async Task<List<BotEmojis>> GetBotEmoji(EmojisType emojiType)
        {
            return await _repoEmojis
                .GetManyQueryable(x => x.EmojisTypeId == emojiType)
                .ToListAsync();
        }

        public async Task<List<BotEmojis>> GetBotEmojis()
        {
            return await _repoEmojis
                .GetAll()
                .ToListAsync();
        }

        public async Task<List<BotEmojis>> GetBotEmoji(string emojiName)
        {
            return await _repoEmojis
                .GetManyQueryable(x => x.EmojisName == emojiName)
                .ToListAsync();
        }
        #endregion
    }
}
