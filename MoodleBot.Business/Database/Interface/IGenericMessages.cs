using MoodleBot.Business.Entity;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public interface IGenericMessages
    {
        Task<Dictionary<MessageName, BotMessages>> GetGenericMessageByTypeId(string languageCode, MessageType messageType);
        Task<Dictionary<MessageName, BotMessages>> GetGenericMessage(string languageCode);
    }
}
