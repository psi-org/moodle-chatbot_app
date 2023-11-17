namespace MoodleBot.Business.Entity
{
    public class QuizAnswerMessage
    {
        public string AnswerStatus { get; set; }
        public string AnswerFeed { get; set; }
        public bool ShowAnswerStatus { get; set; }
        public bool ShowAnswerFeedback { get; set; }
        public bool IsLastQuestion { get; set; }
        public bool IsAnswerDetailFound { get; set; }
    }
}
