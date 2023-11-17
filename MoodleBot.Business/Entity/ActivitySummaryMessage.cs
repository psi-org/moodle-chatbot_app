using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class ActivitySummaryMessage
    {
        public List<SummaryMessage> Message { get; set; }
        public Dictionary<int, ActivitySummaryAction> NextActionIdOption { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
    }

    public class SummaryMessage
    {
        public string Message { get; set; }
        public bool IsMediaType { get; set; }
    }
}
