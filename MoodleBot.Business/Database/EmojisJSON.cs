using iText.Kernel;
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
    public class EmojisJSON : IEmojis
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private string emojiFileRoute = "./DBTables/BotEmojis.json";
        #endregion

        #region Constructor
        public EmojisJSON(IServiceProvider services, ILogger logger)
        {
            _logger = logger;
            _services = services;
        }

        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojisType emojiType)
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var emoji = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                string text = File.ReadAllText(@""+ emojiFileRoute +"");
                var emojis = JsonSerializer.Deserialize<List<BotEmojis>>(text);
                var emojiDetail = emojis.Where(emoji => emoji.EmojisTypeId == emojiType).ToList();
                emoji = emojiDetail.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmoji: Got an error while get emoji", exception);
            }

            return emoji;
        }

        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmoji(EmojiName emojiName)
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var emoji = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                string text = File.ReadAllText(@""+ emojiFileRoute +"");
                var emojis = JsonSerializer.Deserialize<List<BotEmojis>>(text);
                var emojiDetail = emojis.Where(emoji => emoji.EmojisName == emojiName.ToString()).ToList();
                emoji = emojiDetail.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmoji: Got an error while get emoji", exception);
            }

            return emoji;
        }

        public async Task<Dictionary<EmojiName, BotEmojis>> GetEmojis()
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var emoji = new Dictionary<EmojiName, BotEmojis>();
            try
            {
                string text = File.ReadAllText(@""+ emojiFileRoute +"");
                var emojis = JsonSerializer.Deserialize<List<BotEmojis>>(text);
                emoji = emojis.ToDictionary(x => (EmojiName)Enum.Parse(typeof(EmojiName), x.EmojisName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetEmoji: Got an error while get emoji", exception);
            }

            return emoji;
        }
        #endregion

        #region Public Method

    }
    #endregion
}
