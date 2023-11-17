using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public class GenericMessageStore : IGenericMessageStore
    {
        #region Properties
        private readonly IRepositoryBase<BotMessages> _repoBotMessages;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public GenericMessageStore(IRepositoryBase<BotMessages> repoBotMessages, IConfiguration configuration)
        {
            _repoBotMessages = repoBotMessages;
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<List<BotMessages>> GetBotMessagesByTypeId(string languageCode, MessageType messageType)
        {
            var messages = await _repoBotMessages
                .GetManyQueryable(x => x.LanguageISOCode == languageCode && x.TypeId == messageType)
                .ToListAsync();

            if (messages == null || messages.Count <= 0)
                messages = await GetBotMessagesByTypeId(_configuration.GetMoodleConfig("DefaultLanguageCode"), messageType);

            return messages;
        }

        public async Task<List<BotMessages>> GetBotMessages(string languageCode)
        {
            var messages = await _repoBotMessages
                .GetManyQueryable(x => x.LanguageISOCode == languageCode)
                .ToListAsync();

            if (messages == null || messages.Count <= 0)
                messages = await GetBotMessages(_configuration.GetMoodleConfig("DefaultLanguageCode"));

            return messages;
        }
        #endregion
    }
}
