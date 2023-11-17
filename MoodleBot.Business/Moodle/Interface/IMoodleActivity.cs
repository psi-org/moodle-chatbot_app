using MoodleBot.Business.Entity;
using MoodleBot.Common.Enums;
using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public interface IMoodleActivity
    {
        Task<ActivityMessage> GetActivityMessageAsync(long userId, long courseId, int selectedCourseOption, int pageNumber, string languageCode);
        Task<ActivityMessage> GetActivityConfirmationMessageAsync(long userId, long courseId, long activityId, int selectedActivityOption, string languageCode);
        Task<List<MoodleActivityDetail>> GetActivity(long userId, long courseId);
        Task<QuizQuestionMessage> GetQuizQuestionAsync(long userId, long quizId, string languageCode, long? activityId = null);
        Task<ActivitySummaryMessage> GetActivitySummaryAsync(long userId, ActivityType activityTypeId, bool IsLastActivity, long quizAttemptId, string languageCode, long courseId, bool shouldShowCertificate);
        Task<QuizAnswerMessage> PostQuizAnswer(long userId, long quizAttemptsId, long answerId, long questionAttemptId, long currentPage);
        Task<LessonActivityMessage> GetLessonActivityDetailAsync(long userId, long lessonId, long? pageId, long? currentPage, bool shouldReturnGenericMessage, string languageCode);
        Task<FeedbackQuestionMessage> GetFeedbackQuestionAsync(long userId, long activityId, long feedbackItemId, string languageCode);
        Task<FeedbackAnswerMessage> PostFeedbackAnswer(long userId, long activityId, long feedbackItemId, string answer);
    }
}
