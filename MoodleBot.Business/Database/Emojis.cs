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
    public class Emojis : IEmojis
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        #endregion

        #region Constructor
        public Emojis(IServiceProvider services, ILogger logger)
        {
            _logger = logger;
            _services = services;
        }
        #endregion

        #region Public Method
        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojisType emojiType)
        {
            var emojis = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                using var scope = _services.CreateScope();
                var emojisStore = scope.ServiceProvider.GetRequiredService<IEmojisStore>();
                var emojiDetail = await emojisStore.GetBotEmoji(emojiType);
                emojis = emojiDetail.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmoji: Got an error while get emoji", exception);
            }

            return emojis;
        }

        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojiName emojiType)
        {
            var emojis = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                using var scope = _services.CreateScope();
                var emojisStore = scope.ServiceProvider.GetRequiredService<IEmojisStore>();
                var emojiDetail = await emojisStore.GetBotEmoji(emojiType.ToString());
                emojis = emojiDetail.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmoji: Got an error while get emoji", exception);
            }

            return emojis;
        }

        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmojis()
        {
            var emojis = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                using var scope = _services.CreateScope();
                var emojisStore = scope.ServiceProvider.GetRequiredService<IEmojisStore>();
                var emojiDetail = await emojisStore.GetBotEmojis();
                emojis = emojiDetail.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmojis: Got an error while get emojis", exception);
            }

            return emojis;
        }
        #endregion
    }
}
