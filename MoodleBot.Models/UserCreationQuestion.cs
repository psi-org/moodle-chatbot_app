using MoodleBot.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class UserCreationQuestion : BaseEntity
    {
        public string MessageName { get; set; }
        public string Question { get; set; }
        public CreateUserMessageType MessageType { get; set; }
        public int MessagePosition { get; set; }
        public bool IsRequired { get; set; }
        public string ValidationName { get; set; }
        public string ValidationMessage { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string LanguageISOCode { get; set; }
    }
}
