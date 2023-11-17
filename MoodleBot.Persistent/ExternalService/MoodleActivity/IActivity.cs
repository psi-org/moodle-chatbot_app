using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public interface IActivity
    {
        Task<List<MoodleActivityDetail>> GetActivity(long userId, long courseId, long? activityId = null, long? CourseActivityId = null);

        Task<MoodleGetNextQuestionDetail> GetNextQuestion(long userId, long quizId);

        Task<MoodlePostQuizAnswerDetail> PostQuizAnswer(long userId, long quizAttemptsId, long answerId, long questionAttemptId, long currentPage);

        Task<MoodleQuizActivitySummaryDetail> GetQuizActivitySummary(long quizAttemptsId);
        
        Task<MoodleOpenLessonActivityDetail> GetOpenLessonActivity(long userId, long LessonId);

        Task<MoodleLessonActivityDetail> GetLessonActivity(long userId, long PageId, long currentPage);

        Task<MoodleLessonActivitySummaryDetail> GetLessonActivitySummary(long userId, long LessonId);

        Task<MoodleFeedbackQuestionDetail> GetFeedbackActivityQuestion(long userId, long feedbackId);

        Task<MoodlePostFeedbackAnswerDetail> PostFeedbackActivityAnswer(long userId, long feedbackQuestionId, string answer);
    }
}
