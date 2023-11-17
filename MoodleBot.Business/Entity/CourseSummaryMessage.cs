using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class CourseSummaryMessage
    {
        public string Message { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
    }
}
