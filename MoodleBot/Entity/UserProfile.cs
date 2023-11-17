using MoodleBot.Business.Entity;
using MoodleBot.Common.Enums;
using System.Collections.Generic;

namespace MoodleBot.Entity
{
    public class UserProfile
    {
        public long UserId { get; set; }
        public bool IsWrongInputAdded { get; set; }
        public int InValidInputAttempt { get; set; }
        public string WhatsAppNumber { get; set; }
        public bool IsVerifiedUser { get; set; }
        public Persistent.Entity.MoodleUserDetail UserDetail { get; set; }
        public CourseDetail CourseDetail { get; set; }
        public ActivityDetail ActivityDetail { get; set; }
        public LessonActivityDetail LessonActivityDetail { get; set; }
        public QuizActivityDetail QuizActivityDetail { get; set; }
        public FeedbackActivityDetail FeedbackActivityDetail { get; set; }
        public bool ShouldDisplayNextQuestion { get; set; }
        public bool ShouldDisplayWelComeMessage { get; set; }
        public CreateUserAccountDetail CreateUserAccountDetail { get; set; }
        public bool IsUserCreated { get; set; }
        public string PreferredLanguageCode { get; set; }
    }

    public class CreateUserAccountDetail
    {
        public Dictionary<string, string> UserDetails { get; set; }
        //public string UserId { get; set; }
        public string WhatsAppId { get; set; }
        public string CountryCode { get; set; }
        public string CurrentLanguageCode { get; set; }
        public string MobileCountryCode { get; set; }
        public string MobilePhoneNumber { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public List<int> LanguageChoiceAction { get; set; }
        public string UserCountryCode { get; set; }
        public string QuestionValidationName { get; set; }
        public string CurrentQuestionName { get; set; }
        public bool IsLastQuestion { get; set; }
        public bool ShouldStopAccountCreationProcess { get; set; }
    }

    public class CourseDetail
    {
        public Dictionary<int, CourseDetailDto> CourseIdOptionMapping { get; set; }
        public bool ShouldReturnOnCoursePage { get; set; }
        public int CurrentCourseId { get; set; }
        public string CurrentCourseName { get; set; }
        public string CurrentCourseImageUrl { get; set; }
        public bool ShouldShowCourseSummary { get; set; }
        public bool ShouldContinueCourse { get; set; }
        public int CurretPageNumber { get; set; }
        public int LastPageNumber { get; set; }
        public bool ShouldLoadFirstPage { get; set; }
    }

    public class ActivityDetail
    {
        public Dictionary<int, int> ActivityIdOptionMapping { get; set; }
        public int CurrentActivityId { get; set; }
        public ActivityType CurrentActivityTypeId { get; set; }
        public string CurrentActivityName { get; set; }
        public string CurrentActivityImgUrl { get; set; }
        public int CurrentActivitySelectedOption { get; set; }
        public bool ShouldStartActivity { get; set; }
        public bool IsLastActivity { get; set; }
        public long CurrentActivityInstanceId { get; set; }
        public bool ShouldReturnOnActivityPage { get; set; }
        public int CurretPageNumber { get; set; }
        public int LastPageNumber { get; set; }
        public bool ShouldLoadFirstPage { get; set; }
        public Dictionary<int, ActivitySummaryAction> NextActionIdOption { get; set; }
        public bool ShouldShowCertificate { get; set; }
        public bool ShouldReturnOnActivitySummary { get; set; }
    }

    public class LessonActivityDetail
    {
        public string LessonActionMessage { get; set; }
        public string InvalidInputMessage { get; set; }
        public string UserGreetingsOnActivityComplete { get; set; }
        public bool IsGenericMessageAvailable { get; set; }
        public long CurrentPage { get; set; }
        public long PageId { get; set; }
        public bool ShouldShowSummary { get; set; }

    }

    public class QuizActivityDetail
    {
        public long? QuizAttemptsId { get; set; }
        public long? QuestionAttemptId { get; set; }
        public int? CurrentPage { get; set; }
        public Dictionary<int, AnswerDetail> QuizIdOptionMapping { get; set; }
    }

    public class FeedbackActivityDetail
    {
        public long FeedbackItemId { get; set; }
        public FeedbackQuestionType QuestionType { get; set; }
        public Dictionary<int, FeedbackChoiceDetail> AnswerIdOptionMapping { get; set; }
    }
}
