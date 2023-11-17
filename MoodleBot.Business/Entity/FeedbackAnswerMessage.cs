namespace MoodleBot.Business.Entity
{
    public class FeedbackAnswerMessage
    {
        public string ErrorMessage { get; set; }
        public bool IsLastQuestion { get; set; }
        public bool IsAnswerDetailFound { get; set; }
    }
}
