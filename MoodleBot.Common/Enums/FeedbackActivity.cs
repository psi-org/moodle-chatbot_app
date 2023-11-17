using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum FeedbackQuestionType
    {
        [Description("multichoice")]
        MultiChoice = 1,
                
        [Description("multichoicerated")]
        MultiChoiceCerated = 2,

        [Description("textfield")]
        TextField = 3
    }
}
