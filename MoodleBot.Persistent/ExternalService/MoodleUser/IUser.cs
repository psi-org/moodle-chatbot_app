using MoodleBot.Persistent.Entity;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public interface IUser
    {
        Task<MoodleUserDetail> GetUserDetail(string mobilePhone, long? userId = null);
        Task<MoodleCreateUserDetail> CreateNewUserInMoodle(MoodleCreateUserRequest moodleCreateUserRequest);
        Task<MoodleUpdateUserDetail> UpdateUserInMoodle(MoodleUpdateUserRequest moodleUpdateUserRequest);
    }
}
