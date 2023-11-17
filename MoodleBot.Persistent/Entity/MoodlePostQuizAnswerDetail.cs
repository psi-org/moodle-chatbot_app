using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodlePostQuizAnswerDetail
    {
        [JsonProperty("answerstatus")]
        public string AnswerStatus;

        [JsonProperty("answerfeed")]
        public string AnswerFeed;

        [JsonProperty("showanswerstatus")]
        public bool ShowAnswerStatus;

        [JsonProperty("showanswerfeedback")]
        public bool ShowAnswerFeedback;

        [JsonProperty("islastquestion")]
        public bool IsLastQuestion;
    }

    public class PostQuizAnswerDetailRequest : MoodleAPIBaseParameter
    {
        public PostQuizAnswerDetailRequest(IConfiguration configuration) : base(configuration) { }

        [JsonProperty("quizattemptsid")]
        public long QuizAttemptsId { get; set; }

        [JsonProperty("answerid")]
        public long AnswerId { get; set; }

        [JsonProperty("questionattemptid")]
        public long QuestionAttemptId { get; set; }
        
        [JsonProperty("currentpage")]
        public long CurrentPage { get; set; }
    }
}
