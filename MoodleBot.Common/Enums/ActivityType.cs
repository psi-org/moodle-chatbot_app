using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum ActivityType
    {
        [Description("Lesson")]
        Lesson = 15,

        [Description("Quiz")]
        Quiz = 18,

        [Description("Feedback")]
        Feedback = 8,
    }

    public enum ActivitySummaryAction
    {
        [Description("CourseMenu")]
        CourseMenu = 1,

        [Description("CourseSummary")]
        CourseSummary = 2,

        [Description("NextActivity")]
        NextActivity = 3,

        [Description("RepetActivity")]
        RepetActivity = 4
    }
}
