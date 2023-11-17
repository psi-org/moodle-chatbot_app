using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum MessageType
    {
        [Description("WelcomeMessage")]
        WelcomeMessage = 1,

        [Description("User")]
        User = 2,

        [Description("Course")]
        Course = 3,

        [Description("Activity")]
        Activity = 4,

        [Description("Lesson")]
        Lesson = 5,

        [Description("Quiz")]
        Quiz = 6,

        [Description("Error")]
        Error = 7,

        [Description("Pagination")]
        Pagination = 8,

        [Description("Feedback")]
        Feedback = 9
    }

    public enum MessageName
    {
        WELCOME_MESSAGE,
        WELCOME_IMAGE,
        USER_WELCOME,
        USER_NOT_REGISTERD,
        USER_NOT_VERIFIED,
        USER_GREETINGS,
        USER_GREETINGS_ON_ACTIVITY_COMPLETE,
        COURSE_LIST_INFO,
        COURSE_LIST,
        COURSE_NOT_FOUND,
        COURSE_SUMMARY,
        COURSE_ACTION_CURRENT_COURSE,
        COURSE_ACTION_SHOW_MENU,
        ACTIVITY_LIST_INFO,
        ACTIVITY_LIST,
        ACTIVITY_NOT_FOUND,
        ACTIVITY_GO_BACK_TO_COURSE,
        ACTIVITY_SELECTION_CONFIRM_TITLE,
        ACTIVITY_SELECTION_CONFIRM,
        ACTIVITY_SUMMARY,
        SUMMARY_ACTION,
        ACTIVITY_ACTION_COURSE_MENU,
        ACTIVITY_ACTION_SHOW_COURSE_SUMMARY,
        ACTIVITY_ACTION_REPEAT_ACTIVITTY,
        ACTIVITY_ACTION_NEXT_ACTIVITY,
        GO_BACK_TO_ACTIVITY,
        LESSON_ACTION_CONTINUE,
        LESSON_ACTION_GO_BACK_TO_ACTIVITY,
        QUESTIION_ATTEMPT_COUNT,
        QUESTIION_TEMPLATE,
        QUESTIION_OPTION_TEMPLATE,
        QUIZ_SUMMARY,
        INVALID_INPUT,
        PAGE_NEXT,
        PAGE_PREVIOUS,
        FEEDBACK_ACTIVITY_ALREADY_COMPLETED,
        DOWNLOAD_CERTIFICATE,
        CERTIFICATE_WAITING_MESSAGE,
        CERTIFICATE_NOT_FOUND,
        OPTIONAL
    }
}
