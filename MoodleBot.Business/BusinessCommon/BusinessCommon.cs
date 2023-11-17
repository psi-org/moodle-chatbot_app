using Microsoft.Extensions.Configuration;
using MoodleBot.Business.Database;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoodleBot.Business
{
    public class BusinessCommon : IBusinessCommon
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IEmojis _emojis;
        private readonly IGenericMessages _genericMessages;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public BusinessCommon(IGenericMessages genericMessages, IEmojis emojis, ILogger logger, IConfiguration configuration)
        {
            _emojis = emojis;
            _genericMessages = genericMessages;
            _logger = logger;
            _configuration = configuration;
        } 
        #endregion

        #region Pubilc Method

        public async Task<(int lastPageNumber, List<TList> result)> GetPageWiseRecord<TList>(List<TList> lists, int pageNumber, int pageSize)
        {
            List<TList> result = null;
            var lastPageNumber = 0;
            if (lists?.Count > 0)
            {
                await Task.Run(() =>
                {
                    pageNumber--;
                    result = lists.Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    lastPageNumber = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(lists.Count) / pageSize));
                });
            }

            return (lastPageNumber, result);
        }

        public async Task<string> GetPaginationMessage(int pageNumber, int lastPageNumber, string languageCode)
        {
            var result = string.Empty;
            try
            {
                var botMessages = await _genericMessages.GetGenericMessageByTypeId(languageCode, MessageType.Pagination);
                var botEmojis = await _emojis.GetEmoji(EmojisType.Pagination);

                if (pageNumber != lastPageNumber)
                {
                    result = string.Format(botMessages[MessageName.PAGE_NEXT].Message, botEmojis[EmojiName.PAGE_NEXT].Emojis);
                }

                if (pageNumber != 1)
                {
                    result += string.Format(botMessages[MessageName.PAGE_PREVIOUS].Message, botEmojis[EmojiName.PAGE_PREVIOUS].Emojis);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetPaginationMessage: Got an error while prepraing PaginationMessage", exception);
            }

            return result;
        }

        public async Task<Dictionary<EmojiName, string>> GetPaginationOption(int pageNumber, int lastPageNumber)
        {
            var result = new Dictionary<EmojiName, string>();
            try
            {
                var botEmojis = await _emojis.GetEmoji(EmojisType.Pagination);

                if (pageNumber != lastPageNumber)
                {
                    result.Add(EmojiName.PAGE_NEXT, botEmojis[EmojiName.PAGE_NEXT].Emojis.ToLower());
                }

                if (pageNumber != 1)
                {
                    result.Add(EmojiName.PAGE_PREVIOUS, botEmojis[EmojiName.PAGE_PREVIOUS].Emojis.ToLower());
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetPaginationOption: Got an error while prepraing PaginationMessage", exception);
            }

            return result;
        }

        #endregion
    }
}
