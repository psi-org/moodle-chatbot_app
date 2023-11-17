using MoodleBot.Business.Entity;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business
{
    public interface IMoodleUser
    {
        Task<MoodleUserDetail> GetUserDetail(string whatsAppId, long? userId = null);
        Task<WhatsAppNumberDetailMessage> GetWhatsAppNumberDetailsAsync(string whatsAppId, string languageCode);
        Task<WhatsAppNumberDetailMessage> GetLanguageSelectionMessage(string isoCode, string languageCode);
        Task<CreateUserQuestionMessage> GetNextQuestion(int currentQuestion, Dictionary<string, string> userDetails, string languageCode);
        Task<bool> CreateUserInMoodle(CreateUserAccountRequest userAccountRequest);
        Task<bool> UpdateUserInMoodle(CreateUserAccountRequest userAccountRequest);
        Task<Dictionary<CreateUserMessageName, UserCreationQuestion>> GetQuestionDetails(string languageCode);
    }
}
