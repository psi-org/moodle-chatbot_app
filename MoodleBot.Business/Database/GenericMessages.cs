using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public class GenericMessages : IGenericMessages
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        #endregion

        #region Constructor
        public GenericMessages(ILogger logger, IServiceProvider services)
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
                using var scope = _services.CreateScope();
                var genericMessageStore = scope.ServiceProvider.GetRequiredService<IGenericMessageStore>();
                var messageDetail = await genericMessageStore.GetBotMessagesByTypeId(languageCode, messageType);
                genericMessage = messageDetail.ToDictionary(x => (MessageName)Enum.Parse(typeof(MessageName), x.MessageName), x => x);
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
                using var scope = _services.CreateScope();
                var genericMessageStore = scope.ServiceProvider.GetRequiredService<IGenericMessageStore>();
                var messageDetail = await genericMessageStore.GetBotMessages(languageCode);
                genericMessage = messageDetail.ToDictionary(x => (MessageName)Enum.Parse(typeof(MessageName), x.MessageName), x => x);
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
