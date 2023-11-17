using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class ActivityMessage
    {
        public string Message { get; set; }
        public bool IsActivityAvailable { get; set; }
        public bool IsLastActivity { get; set; }
        public ActivityType CurrentActivityTypeId { get; set; }
        public Dictionary<int, int> ActivityIdOptionMapping { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
        public string CurrentActivityName { get; set; }
        public string CurrentActivityImgUrl { get; set; }
        public long CurrentActivityInstanceId { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public bool IsFeedbackActivityCompleted { get; set; }
        public bool IsAccessRestriction { get; set; }
        public bool ShouldShowCertificate { get; set; }
    }
}
