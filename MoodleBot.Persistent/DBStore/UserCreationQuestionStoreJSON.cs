using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public class UserCreationQuestionStoreJSON : IUserCreationQuestionStore
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IRepositoryBase<UserCreationQuestion> _repoUserCreationQuestion;
        private readonly IConfiguration _configuration;
        private string MessagesFileRoute = "./DBTables/UserCreationQuestions.json";
        private string DefaultLanguageCode = "en";
        #endregion

        #region Public Method

        public async Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode, CreateUserMessageType createUserMessageType)
        {
            var messages = new List<UserCreationQuestion>();

            try
            {

                string text = File.ReadAllText(@"" + MessagesFileRoute + "");
                var messagesFile = JsonSerializer.Deserialize<List<UserCreationQuestion>>(text);

                messages = messagesFile.Where(message => message.LanguageISOCode == languageCode && message.MessageType == createUserMessageType).ToList();

                if (messages != null || messages.Count <= 0)
                {
                    messages = messages.Where(message => message.LanguageISOCode == DefaultLanguageCode && message.MessageType == createUserMessageType).ToList();
                }

            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessageByTypeId: Got an error while get the generic Message", exception);
            }

            return messages;
        }

        public async Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode)
        {
            var messages = new List<UserCreationQuestion>();

            try
            {

                string text = File.ReadAllText(@"" + MessagesFileRoute + "");
                var messagesFile = JsonSerializer.Deserialize<List<UserCreationQuestion>>(text);

                messages = messagesFile.Where(message => message.LanguageISOCode == languageCode).ToList();

                if (messages != null || messages.Count <= 0)
                {
                    messages = messages.Where(message => message.LanguageISOCode == DefaultLanguageCode).ToList();
                }

            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessageByTypeId: Got an error while get the generic Message", exception);
            }

            return messages;
        }
        #endregion
    }
}
