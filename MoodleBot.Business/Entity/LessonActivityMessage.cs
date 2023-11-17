using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class LessonActivityMessage
    {
        public List<LessonMessageDetail> Message { get; set; }
        public string ActionMessage { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessage { get; set; }
        public long CurrentPage { get; set; }
        public long PageId { get; set; }
        public bool IsLessonDetailAvailable { get; set; }
    }

    public class LessonMessageDetail
    {
        public string Message { get; set; }
        public LessonContentType ContentType { get; set; }
    }
}
