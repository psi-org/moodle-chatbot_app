using MoodleBot.Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business
{
    public interface IBusinessCommon
    {
        Task<(int lastPageNumber, List<TList> result)> GetPageWiseRecord<TList>(List<TList> lists, int pageNumber, int pageSize);

        Task<string> GetPaginationMessage(int pageNumber, int lastPageNumber, string languageCode);

        Task<Dictionary<EmojiName, string>> GetPaginationOption(int pageNumber, int lastPageNumber);
    }
}
