using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class FeedbackQuestionMessage
    {
        public string Message { get; set; }
        public FeedbackQuestionType QuestionType { get; set; }
        public bool IsQuestionAvailable { get; set; }
        public string ActivityImageUrl { get; set; }
        public long? FeedbackItemId { get; set; }
        public bool IsActivityCompleted { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
        public Dictionary<int, FeedbackChoiceDetail> AnswerIdOptionMapping { get; set; }
    }

    public class FeedbackChoiceDetail
    {
        public string Answer { get; set; }
        public long? AnswerId { get; set; }
    }
}
