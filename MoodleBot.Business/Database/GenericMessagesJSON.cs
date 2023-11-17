using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public class GenericMessagesJSON : IGenericMessages
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private string MessagesFileRoute = "./DBTables/BotMessages.json";
        private string DefaultLanguageCode = "en";
        #endregion

        #region Constructor
        public GenericMessagesJSON(ILogger logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }
        #endregion

        #region Public Method
        public async Task<Dictionary<MessageName, BotMessages>> GetGenericMessageByTypeId(string languageCode, MessageType messageType)
        {
            var genericMessage = new Dictionary<MessageName, BotMessages>();
            try
            {

                string text = File.ReadAllText(@"" + MessagesFileRoute + "");
                var messagesFile = JsonSerializer.Deserialize<List<BotMessages>>(text);

                var messages = messagesFile.Where(message => message.TypeId == messageType && message.LanguageISOCode == languageCode).ToList();

                if (messages != null || messages.Count <= 0)
                {
                    messages = messages.Where(message => message.TypeId == messageType && message.LanguageISOCode == DefaultLanguageCode).ToList();
                }
                
                genericMessage = messages.ToDictionary(x => (MessageName)Enum.Parse(typeof(MessageName), x.MessageName), x => x);

            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessageByTypeId: Got an error while get the generic Message", exception);
            }

            return genericMessage;
        }

        public async Task<Dictionary<MessageName, BotMessages>> GetGenericMessage(string languageCode)
        {
            var genericMessage = new Dictionary<MessageName, BotMessages>();
            try
            {
                string text = File.ReadAllText(@"" + MessagesFileRoute + "");
                var messagesFile = JsonSerializer.Deserialize<List<BotMessages>>(text);

                var messages = messagesFile.Where(message => message.LanguageISOCode == languageCode).ToList();

                if (messages != null || messages.Count <= 0)
                {
                    messages = messages.Where(message => message.LanguageISOCode == DefaultLanguageCode).ToList();
                }

                genericMessage = messages.ToDictionary(x => (MessageName)Enum.Parse(typeof(MessageName), x.MessageName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessage: Got an error while get the generic Message", exception);
            }

            return genericMessage;
        }
        #endregion
    }
}
