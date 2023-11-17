using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class CourseMessage
    {
        public string Message { get; set; }
        public bool IsCourseAvailable { get; set; }
        public Dictionary<int, CourseDetailDto> CourseIdOptionMapping { get; set; }
        public Dictionary<MessageName, BotMessages> GenericMessages { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
    }

    public class CourseDetailDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseImage { get; set; }
    }
}
