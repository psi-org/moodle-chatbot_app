using MoodleBot.Business.Entity;
using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public interface IMoodleCourse
    {
        Task<CourseMessage> GetCourseMessageAsync(long userId, bool shouldAddWelComeMessage, int pageNumber, string languageCode, long? courseId = null);
        Task<List<MoodleCourseDetail>> GetCourse(long userId, long? courseId = null);
        Task<CourseSummaryMessage> GetCourseSummaryAsync(long userId, long courseId, string languageCode);
    }
}
