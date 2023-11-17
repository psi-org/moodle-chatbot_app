using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class QuizQuestionMessage
    {
        public string Message { get; set; }
        public bool IsQuestionAvailable { get; set; }
        public string ActivityImageUrl { get; set; }
        public string NoOfActivityAttempts { get; set; }
        public long? QuizAttemptsId { get; set; }
        public long? QuestionAttemptId { get; set; }
        public int? CurrentPage { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
        public Dictionary<int, AnswerDetail> AnswerIdOptionMapping { get; set; }
    }

    public class AnswerDetail
    {
        public string Answer { get; set; }
        public long? AnswerId { get; set; }
        public int? IsCorrectAnswer { get; set; }
        public string Feedback { get; set; }
    }
}
