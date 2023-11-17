using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public interface IGenericMessageStore
    {
        Task<List<BotMessages>> GetBotMessagesByTypeId(string languageCode, MessageType messageType);
        Task<List<BotMessages>> GetBotMessages(string languageCode);
    }
}
