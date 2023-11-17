using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum LessonContentType
    {
        [Description("text")]
        Text = 1,
        [Description("image")]
        Image = 2,
        [Description("video")]
        Video = 3
    }
}
