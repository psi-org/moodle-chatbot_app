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
    public class UserCreationQuestionStore : IUserCreationQuestionStore
    {
        #region Properties
        private readonly IRepositoryBase<UserCreationQuestion> _repoUserCreationQuestion;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public UserCreationQuestionStore(IRepositoryBase<UserCreationQuestion> repoUserCreationQuestion, IConfiguration configuration)
        {
            _repoUserCreationQuestion = repoUserCreationQuestion;
            _configuration = configuration;
        }
        #endregion

        #region Public Method

        public async Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode, CreateUserMessageType createUserMessageType)
        {
            var messages = await _repoUserCreationQuestion
                .GetManyQueryable(x => x.LanguageISOCode == languageCode && x.MessageType == createUserMessageType)
                .ToListAsync();

            if (messages == null || messages.Count <= 0)
                messages = await GetUserCreationQuestion(_configuration.GetMoodleConfig("DefaultLanguageCode"), createUserMessageType);

            return messages;
        }

        public async Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode)
        {
            var messages = await _repoUserCreationQuestion
                .GetManyQueryable(x => x.LanguageISOCode == languageCode)
                .ToListAsync();

            if (messages == null || messages.Count <= 0)
                messages = await GetUserCreationQuestion(_configuration.GetMoodleConfig("DefaultLanguageCode"));

            return messages;
        }
        #endregion
    }
}
