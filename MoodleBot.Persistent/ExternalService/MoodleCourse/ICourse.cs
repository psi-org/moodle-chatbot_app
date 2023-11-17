using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public interface ICourse
    {
        Task<List<MoodleCourseDetail>> GetCourses(long userId, long? courseId = null);
    }
}
