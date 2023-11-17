using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public interface IUserCreationQuestionStore
    {
        Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode);

        Task<List<UserCreationQuestion>> GetUserCreationQuestion(string languageCode, CreateUserMessageType createUserMessageType);
    }
}
