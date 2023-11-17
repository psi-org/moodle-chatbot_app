using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public interface IEmojis
    {
        Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojisType emojiType);
        Task<Dictionary<EmojiName, BotEmojis>> GetEmojis();
        Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojiName emojiType);
    }
}
