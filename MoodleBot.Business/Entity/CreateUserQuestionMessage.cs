namespace MoodleBot.Business.Entity
{
    public class CreateUserQuestionMessage
    {
        public string Message { get; set; }

        public int CurrentQuestionNumber { get; set; }

        public string PropmtValidationMessage { get; set; }

        public string NextActionMessage { get; set; }

        public bool IsQuestionFound { get; set; }

        public string QuestionValidationName { get; set; }

        public string QuestionValidationMessage { get; set; }

        public bool IsAnswerRequired { get; set; }

        public string CurrentQuestionName { get; set; }

        public bool IsLastQuestion { get; set; }
    }
}
