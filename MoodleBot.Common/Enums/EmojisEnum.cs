using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum EmojisType
    {
        [Description("CourseStaus")]
        CourseStaus = 1,

        [Description("ActivityStatus")]
        ActivityStatus = 2,

        [Description("Generic")]
        Generic = 3,

        [Description("Pagination")]
        Pagination = 4
    }

    public enum EmojiName
    {
        COURSE_NOT_ENROLLED,
        COURSE_ENROLLED,
        COURSE_IN_PROGRESS,
        COURSE_COMPLETED,
        COURSE_COMPLETED_PASS,
        COURSE_COMPLETED_FAIL,
        ACTIVITY_NOT_STARTED,
        ACTIVITY_IN_PROGRESS,
        ACTIVITY_COMPLETED,
        ACTIVITY_COMPLETED_PASS,
        ACTIVITY_COMPLETED_FAIL,
        STATUS_UNKNOWN,
        GRADE_GREEN,
        GRADE_RED,
        ATTEMPT,
        ANSWER_CORRECT,
        ANSWER_INCORRECT,
        SUMMARY,
        ACTIVITY_FINISHED,
        PAGE_NEXT,
        PAGE_PREVIOUS,
        DOWNLOAD_CERTIFICATE,
        RESTRICTED_ITEM,
        STATUS_ALERT
    }
}
