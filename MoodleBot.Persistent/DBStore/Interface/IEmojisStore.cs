using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public interface IEmojisStore
    {
        Task<List<BotEmojis>> GetBotEmoji(EmojisType emojiType);
        Task<List<BotEmojis>> GetBotEmojis();
        Task<List<BotEmojis>> GetBotEmoji(string emojiName);
    }
}
