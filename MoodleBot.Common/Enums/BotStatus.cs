using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum CourseStatus
    {
        [Description("not enrolled")]
        NotEnrolled = 1,
        [Description("enrolled")]
        Enrolled = 2,
        [Description("in progress")]
        InProgress = 3,
        [Description("completed")]
        Completed = 4,
        [Description("completed pass")]
        CompletedPass = 5,
        [Description("completed fail")]
        CompletedFail = 6,
        [Description("unknown")]
        Unknown = 7
    }

    public enum ActivityStatus
    {
        [Description("not started")]
        NotStarted = -1,
        [Description("in progress")]
        InProgress = 0,
        [Description("completed")]
        Completed = 1,
        [Description("complete pass")]
        CompletePass = 2,
        [Description("complete fail")]
        CompleteFail = 3
    }
}
